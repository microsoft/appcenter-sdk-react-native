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
    public class NetworkUnavailableException : IngestionException
    {
    }

    public class NetworkStateIngestion : IngestionDecorator
    {
        private readonly HashSet<IServiceCall> _calls = new HashSet<IServiceCall>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        private bool IsConnected => NetworkInterface.GetIsNetworkAvailable();

        public NetworkStateIngestion(IIngestion decoratedApi) : base(decoratedApi)
        {
            NetworkChange.NetworkAddressChanged += HandleNetworkAddressChanged;
        }

        private async void HandleNetworkAddressChanged(object sender, EventArgs e)
        {
            _mutex.Wait();
            try
            {
                if (IsConnected)
                {
                    var callsCopy = new HashSet<IServiceCall>(_calls);
                    foreach (var call in callsCopy)
                    {
                        _calls.Remove(call);
                        call.Execute();
                    }
                }
                else
                {
                    foreach (var call in _calls)
                    {
                        PauseServiceCall(call);
                    }
                }
            }
            finally
            {
                _mutex.Release();
            }
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

        private void PauseServiceCall(IServiceCall call)
        {
            call?.Cancel();
        }

        public override IServiceCall PrepareServiceCall(string appSecret, Guid installId, IList<Log> logs)
        {
            var call = base.PrepareServiceCall(appSecret, installId, logs);
            return new NetworkStateServiceCall(call, this);
        }

        /// <exception cref="NetworkUnavailableException"/>
        public override async Task ExecuteCallAsync(IServiceCall call)
        {
            await _mutex.WaitAsync();
            _calls.Add(call);
            _mutex.Release();

            if (IsConnected)
            {
                try
                {
                    await base.ExecuteCallAsync(call);
                }
                finally
                {
                    RemoveCall(call);
                }
            }
            else
            {
                throw new NetworkUnavailableException();
            }
        }
        private void RemoveCall(IServiceCall call)
        {
            _mutex.Wait();
            _calls.Remove(call);
            _mutex.Release();
        }
    }
}
