using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Azure.Mobile.Ingestion.Models;
using System.Threading;
using Microsoft.Rest;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class IngestionNetworkStateHandler : IngestionDecorator
    {
        private readonly HashSet<ServiceCall> _calls = new HashSet<ServiceCall>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public IngestionNetworkStateHandler(IIngestion decoratedApi) : base(decoratedApi)
        {
            NetworkChange.NetworkAddressChanged += HandleNetworkAddressChanged;
        }

        private void HandleNetworkAddressChanged(object sender, EventArgs e)
        {
            _mutex.Wait();
            var connected = NetworkInterface.GetIsNetworkAvailable();
            foreach (var call in _calls)
            {
                if (connected)
                {
                    //TODO take some action if the call fails?
                    call.RunWithRetriesAsync().ContinueWith(completedTask =>
                    {
                        if (completedTask.Exception != null)
                        {
                            RemoveCall(call);
                        }
                    });
                }
                else
                {
                    PauseServiceCall(call);
                }
            }
            _mutex.Release();
        }

        public override void Close()
        {
            //TODO what if already closed?
            _mutex.Wait();
            NetworkChange.NetworkAddressChanged -= HandleNetworkAddressChanged;
            foreach (var call in _calls)
            {
                PauseServiceCall(call);
            }
            _calls.Clear();
            _mutex.Release();
            base.Close();
        }

        private void PauseServiceCall(ServiceCall call)
        {
            call?.Cancel();
        }

        ///<exception cref="IngestionException"/>
        public override async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            var call = new ServiceCall(DecoratedApi, logs, appSecret, installId);
            try
            {
                await _mutex.WaitAsync();
                _calls.Add(call);
                _mutex.Release();
                await call.RunWithRetriesAsync();
            }
            finally
            {
                RemoveCall(call);
            }
        }

        private void RemoveCall(ServiceCall call)
        {
            _mutex.Wait();
            if (_calls.Contains(call))
            {
                _calls.Remove(call);
            }
            _mutex.Release();
        }

        class ServiceCall
        {
            private static readonly Random Random = new Random();
            private static readonly SemaphoreSlim RandomMutex = new SemaphoreSlim(1, 1);

            private readonly IList<Log> _logs;
            private CancellationTokenSource _tokenSource;
            private readonly IIngestion _ingestion;
            private readonly string _appSecret;
            private readonly Guid _installId;
            private int _retryCount;
            private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

            private readonly TimeSpan[] _retryIntervals = { TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(20) };
            public ServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
            {
                _ingestion = ingestion;
                _logs = logs;
                _appSecret = appSecret;
                _installId = installId;
            }

            ///<exception cref="IngestionException"/>
            public async Task RunWithRetriesAsync()
            {
                await _mutex.WaitAsync();
                try
                {
                    await RunWithRetriesAsyncHelper();
                }
                finally
                {
                    _tokenSource = null;
                    _mutex.Release();
                }
            }

            ///<exception cref="IngestionException"/>
            private async Task RunWithRetriesAsyncHelper()
            {
                _tokenSource = _tokenSource ?? new CancellationTokenSource();
                try
                {
                    await _ingestion.SendLogsAsync(_appSecret, _installId, _logs, _tokenSource.Token);
                }
                catch (HttpOperationException exception)
                {
                    var ingestionException = new IngestionException(exception);
                    await _mutex.WaitAsync();
                    if (!HttpUtils.IsRecoverableError(ingestionException) || _retryCount >= _retryIntervals.Length)
                    {
                        _mutex.Release();
                        throw ingestionException;
                    }

                    var delayMilliseconds = (int)(_retryIntervals[_retryCount++].TotalMilliseconds / 2.0);
                    delayMilliseconds += await GetRandomIntAsync(delayMilliseconds);
                    var message = $"Try #{_retryCount} failed and will be retried in {delayMilliseconds} ms";
                    _mutex.Release();
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, message, exception); //TODO unknown host stuff?
                    await Task.Delay(delayMilliseconds);
                    _tokenSource.Token.ThrowIfCancellationRequested();
                    await RunWithRetriesAsyncHelper();
                }
            }

            private static async Task<int> GetRandomIntAsync(int max)
            {
                await RandomMutex.WaitAsync();
                var randomInt = Random.Next(max);
                RandomMutex.Release();
                return randomInt;
            }

            public void Cancel()
            {
                _tokenSource?.Cancel();
            }
        }
    }
}
