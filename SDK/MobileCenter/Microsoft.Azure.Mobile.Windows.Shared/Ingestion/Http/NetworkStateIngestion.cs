using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkUnavailableException : IngestionException
    {
    }

    public class NetworkStateIngestion : IngestionDecorator
    {
        private readonly HashSet<IServiceCall> _calls = new HashSet<IServiceCall>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
		private readonly INetworkStateAdapter _networkState;
		
		public NetworkStateIngestion(IIngestion decoratedApi) :
			this(decoratedApi, new NetworkStateAdapter())
		{
		}

		public NetworkStateIngestion(IIngestion decoratedApi, INetworkStateAdapter networkState)
			: base(decoratedApi)
        {
			_networkState = networkState;
			_networkState.NetworkAddressChanged += HandleNetworkAddressChanged;
        }

        private void HandleNetworkAddressChanged(object sender, EventArgs e)
        {
            _mutex.Wait();
            try
            {
                if (_networkState.IsConnected)
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
			_networkState.NetworkAddressChanged -= HandleNetworkAddressChanged;
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

            if (_networkState.IsConnected)
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
