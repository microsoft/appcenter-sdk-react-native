using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Azure.Mobile.UWP.Ingestion.Models;
using System.Threading;
using Microsoft.Rest;
using System.Net;

//TODO thread safety

namespace Microsoft.Azure.Mobile.UWP.Ingestion.Http
{
    public class IngestionNetworkStateHandler : IngestionDecorator
    {
        private HashSet<ServiceCall> _calls = new HashSet<ServiceCall>();

        public IngestionNetworkStateHandler(IIngestion decoratedApi) : base(decoratedApi)
        {
            NetworkChange.NetworkAddressChanged += HandleNetworkAddressChanged;
        }

        private void HandleNetworkAddressChanged(object sender, EventArgs e) //TODO is this right? seems quite strange that this exception isn't going anywhere...maybe at least write a log or something?
        {
            bool connected = NetworkInterface.GetIsNetworkAvailable();
            foreach (var call in _calls)
            {
                if (connected)
                {
                    call.RunWithRetriesAsync().ContinueWith((completedTask) =>
                    {
                        if (completedTask.Exception != null)
                        {
                            _calls.Remove(call);
                        }
                    });
                }
                else
                {
                    PauseServiceCall(call);
                }
            }
        }

        public override void Close()
        {
            NetworkChange.NetworkAddressChanged -= HandleNetworkAddressChanged;
            foreach (var call in _calls)
            {
                PauseServiceCall(call);
            }
            _calls.Clear();
            base.Close();
        }

        private void PauseServiceCall(ServiceCall call)
        {
            call?.Cancel();
        }

        public override async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            var call = new ServiceCall(_decoratedApi, logs, appSecret, installId);
            _calls.Add(call);
            try
            {
                await call.RunWithRetriesAsync();
            }
            finally
            {
                _calls.Remove(call);
            }
        }

        class ServiceCall
        {
            private static Random _random = new Random();

            public IList<Log> _logs;
            private CancellationTokenSource _tokenSource;
            public IIngestion _ingestion;
            public string _appSecret;
            public Guid _installId;
            private int _retryCount = 0;
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
                _tokenSource = _tokenSource ?? new CancellationTokenSource();
                try
                {
                    await _ingestion.SendLogsAsync(_appSecret, _installId, _logs, _tokenSource.Token);
                }
                catch (HttpOperationException exception)
                {
                    if (!HttpUtils.IsRecoverableError(exception) || _retryCount >= RetryIntervals.Length)
                    {
                        throw;
                    }

                    int delayMilliseconds = (int)(RetryIntervals[_retryCount++].TotalMilliseconds / 2.0);
                    delayMilliseconds += _random.Next(delayMilliseconds);
                    string message = "Try #" + _retryCount + " failed and will be retried in " + delayMilliseconds + " ms";
                    MobileCenterLog.Warn(MobileCenterLog.LogTag, message, exception); //TODO unknown host stuff?
                    await Task.Delay(delayMilliseconds);
                    _tokenSource.Token.ThrowIfCancellationRequested();
                    await RunWithRetriesAsync();
                }
            }

            public void Cancel()
            {
                _tokenSource.Cancel();
                _tokenSource = null;
            }
        }
    }
}
