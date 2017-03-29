using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
    public class StatefulMutex : IDisposable
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly StateKeeper _stateKeeper;

        public StatefulMutex(StateKeeper stateKeeper)
        {
            _stateKeeper = stateKeeper;
        }

        public void Lock()
        {
            _mutex.Wait();
        }

        public void Lock(State stateSnapshot)
        {
            _mutex.Wait();
            if (!_stateKeeper.IsCurrent(stateSnapshot))
            {
                throw new StatefulMutexException("Cannot lock mutex with expired state");
            }
        }

        public async Task LockAsync(State stateSnapshot)
        {
            await _mutex.WaitAsync();
            if (!_stateKeeper.IsCurrent(stateSnapshot))
            {
                Unlock();
                throw new StatefulMutexException("Cannot lock mutex with expired state");
            }
        }

        public async Task LockAsync()
        {
            await _mutex.WaitAsync();
        }

        //Double-unlocking does not throw an error
        public void Unlock()
        {
            if (_mutex.CurrentCount == 0)
            {
                _mutex.Release();
            }
        }

        public void Dispose()
        {
            _mutex.Dispose();
        }

    }
}
