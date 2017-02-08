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
        private HashSet<ServiceCall> _calls = new HashSet<ServiceCall>();
        private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public IngestionNetworkStateHandler(IIngestion decoratedApi) : base(decoratedApi)
        {
            NetworkChange.NetworkAddressChanged += HandleNetworkAddressChanged;
        }

        private void HandleNetworkAddressChanged(object sender, EventArgs e) //TODO is this right? seems quite strange that this exception isn't going anywhere...maybe at least write a log or something? Perhaps completion events would be more appropriate here
        {
            _mutex.Wait();
            bool connected = NetworkInterface.GetIsNetworkAvailable();
            foreach (var call in _calls)
            {
                if (connected)
                {
                    call.RunWithRetriesAsync().ContinueWith((completedTask) =>
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

        public override async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            var call = new ServiceCall(_decoratedApi, logs, appSecret, installId);
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
            private static Random _random = new Random();
            private static SemaphoreSlim _randomMutex = new SemaphoreSlim(1, 1);

            private IList<Log> _logs;
            private CancellationTokenSource _tokenSource;
            private IIngestion _ingestion;
            private string _appSecret;
            private Guid _installId;
            private int _retryCount = 0;
            private SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

            private TimeSpan[] RetryIntervals = new TimeSpan[] { TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(20) };
            public ServiceCall(IIngestion ingestion, IList<Log> logs, string appSecret, Guid installId)
            {
                _ingestion = ingestion;
                _logs = logs;
                _appSecret = appSecret;
                _installId = installId;
            }

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

            private async Task RunWithRetriesAsyncHelper()
            {
                _tokenSource = _tokenSource ?? new CancellationTokenSource();
                try
                {
                    await _ingestion.SendLogsAsync(_appSecret, _installId, _logs, _tokenSource.Token);
                }
                catch (HttpOperationException exception)
                {
                    await _mutex.WaitAsync();
                    if (!HttpUtils.IsRecoverableError(exception) || _retryCount >= RetryIntervals.Length)
                    {
                        _mutex.Release();
                        throw;
                    }

                    int delayMilliseconds = (int)(RetryIntervals[_retryCount++].TotalMilliseconds / 2.0);
                    delayMilliseconds += await RandomInt(delayMilliseconds);
                    string message = "Try #" + _retryCount + " failed and will be retried in " + delayMilliseconds + " ms";
                    _mutex.Release();
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, message, exception); //TODO unknown host stuff?
                    await Task.Delay(delayMilliseconds);
                    _tokenSource.Token.ThrowIfCancellationRequested();
                    await RunWithRetriesAsyncHelper();
                }
            }

            private static async Task<int> RandomInt(int max)
            {
                await _randomMutex.WaitAsync();
                int randomInt = _random.Next(max);
                _randomMutex.Release();
                return randomInt;
            }

            public void Cancel()
            {
                _tokenSource?.Cancel();
            }
        }
    }
}
