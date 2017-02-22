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
    public class RetryableIngestion : IngestionDecorator
    {
        private readonly HashSet<RetryableServiceCall> _calls = new HashSet<RetryableServiceCall>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public RetryableIngestion(IIngestion decoratedApi) : base(decoratedApi)
        {
            NetworkChange.NetworkAddressChanged += HandleNetworkAddressChanged;
        }

        private  void HandleNetworkAddressChanged(object sender, EventArgs e)
        {
            _mutex.Wait();
            var connected = NetworkInterface.GetIsNetworkAvailable();
            foreach (var call in _calls)
            {
                if (connected)
                {
                    call.Execute();
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

        private void PauseServiceCall(RetryableServiceCall call)
        {
            call?.Cancel();
        }

        public override IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs,
            CancellationToken cancellationToken = new CancellationToken())
        {
            return new RetryableServiceCall(DecoratedApi, this, logs, appSecret, installId);
        }

        ///<exception cref="IngestionException"/>
        public override async Task SendLogsAsync(string appSecret, Guid installId, IList<Log> logs, CancellationToken cancellationToken = default(CancellationToken))
        {
            var call = new RetryableServiceCall(DecoratedApi, this, logs, appSecret, installId);
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

        private void RemoveCall(RetryableServiceCall call)
        {
            _mutex.Wait();
            if (_calls.Contains(call))
            {
                _calls.Remove(call);
            }
            _mutex.Release();
        }

        
    }
}
