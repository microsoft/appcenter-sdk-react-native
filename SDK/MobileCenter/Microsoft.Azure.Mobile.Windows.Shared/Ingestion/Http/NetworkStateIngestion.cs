using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Mobile.Ingestion.Models;

namespace Microsoft.Azure.Mobile.Ingestion.Http
{
    public class NetworkStateIngestion : IngestionDecorator
    {
        private readonly HashSet<IServiceCall> _calls = new HashSet<IServiceCall>();
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly INetworkStateAdapter _networkStateAdapter;

        public NetworkStateIngestion(IIngestion decoratedApi) :
            this(decoratedApi, new NetworkStateAdapter())
        {
        }

        public NetworkStateIngestion(IIngestion decoratedApi, INetworkStateAdapter networkStateAdapter)
            : base(decoratedApi)
        {
            _networkStateAdapter = networkStateAdapter;
        }

        public override void Close()
        {
            _mutex.Wait();
            foreach (var call in _calls)
            {
                PauseServiceCall(call);
            }
            _calls.Clear();
            _mutex.Release();
            base.Close();
        }

        internal async Task WaitAllCalls()
        {
            int callsCount;
            do
            {
                await _mutex.WaitAsync().ConfigureAwait(false);
                callsCount = _calls.Count;
                _mutex.Release();
            } while (callsCount > 0);
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

        public override async Task ExecuteCallAsync(IServiceCall call)
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            _calls.Add(call);
            _mutex.Release();
            try
            {
                await ExecuteCallAsyncHelper(call);
            }
            finally
            {
                // Regardless of success, the call must be removed from the collection
                RemoveCall(call);
            }
        }
        private async Task ExecuteCallAsyncHelper(IServiceCall call)
        {
            if (_networkStateAdapter.IsConnected)
            {
                try
                {
                    await base.ExecuteCallAsync(call).ConfigureAwait(false);
                }
                // Recursion case in case network goes out during the base ExecuteCallAsync call
                catch (NetworkIngestionException)
                {
                    // Since we caught a NetworkIngestionException, just wait for network and then retry
                    await PauseExecutionUntilNetworkBecomesAvailable().ConfigureAwait(false);
                    await ExecuteCallAsyncHelper(call).ConfigureAwait(false);
                }
                return;
            }

            // Recursion case
            await PauseExecutionUntilNetworkBecomesAvailable().ConfigureAwait(false);
            await ExecuteCallAsyncHelper(call).ConfigureAwait(false);
        }

        private async Task PauseExecutionUntilNetworkBecomesAvailable()
        {
            // No connection; wait until network returns to continue execution
            var networkSemaphore = new SemaphoreSlim(0);
            void NetworkStateChangeHandler(object sender, EventArgs e)
            {
                if (_networkStateAdapter.IsConnected)
                {
                    networkSemaphore.Release();
                }
            }
            _networkStateAdapter.NetworkStatusChanged += NetworkStateChangeHandler;
            await networkSemaphore.WaitAsync().ConfigureAwait(false);
            _networkStateAdapter.NetworkStatusChanged -= NetworkStateChangeHandler;
        }

        private void RemoveCall(IServiceCall call)
        {
            _mutex.Wait();
            _calls.Remove(call);
            _mutex.Release();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _mutex.Dispose();
            }
        }
    }
}
