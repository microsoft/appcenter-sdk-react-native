using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
    /// <summary>
    /// Allows synchronization across tasks, regardless of the thread executing it.
    /// Doing so involves tracking a state, so that once invalidated, trying to 
    /// acquire the lock will fail.
    /// </summary>
    public class StatefulMutex : IDisposable
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private readonly StateKeeper _stateKeeper;

        /// <summary>
        /// Creates a <see cref="StatefulMutex"/>  with an existing <see cref="StateKeeper"/> 
        /// </summary>
        /// <param name="stateKeeper">Tracks the current state for the mutex</param>
        public StatefulMutex(StateKeeper stateKeeper)
        {
            _stateKeeper = stateKeeper;
        }

        /// <summary>
        /// Locks the mutex and does not verify any state
        /// </summary>
        /// <seealso cref="LockAsync"/>
        public void Lock()
        {
            _mutex.Wait();
        }

        /// <summary>
        /// Locks the mutex if the given <see cref="State"/> is consistent with the mutex's <see cref="StateKeeper"/> state
        /// </summary>
        /// <param name="stateSnapshot">The <see cref="State"/> that must be consistent with the mutex's <see cref="StateKeeper"/> state</param>
        /// <exception cref="StatefulMutexException">The given state is invalid</exception>
        /// <seealso cref="LockAsync(State)"/> 
        public void Lock(State stateSnapshot)
        {
            _mutex.Wait();
            if (!_stateKeeper.IsCurrent(stateSnapshot))
            {
                throw new StatefulMutexException("Cannot lock mutex with expired state");
            }
        }

        /// <summary>
        /// Locks the mutex if the given <see cref="State"/> is consistent with the mutex's <see cref="StateKeeper"/> state
        /// </summary>
        /// <param name="stateSnapshot">The <see cref="State"/> that must be consistent with the mutex's <see cref="StateKeeper"/> state</param>
        /// <exception cref="StatefulMutexException">The given state is invalid</exception>
        /// <seealso cref="Lock(State)"/> 
        public async Task LockAsync(State stateSnapshot)
        {
            await _mutex.WaitAsync();
            if (!_stateKeeper.IsCurrent(stateSnapshot))
            {
                Unlock();
                throw new StatefulMutexException("Cannot lock mutex with expired state");
            }
        }

        /// <summary>
        /// Asynchronously locks the mutex and does not verify any state
        /// </summary>
        /// <seealso cref="Lock"/>
        public async Task LockAsync()
        {
            await _mutex.WaitAsync();
        }

        /// <summary>
        /// Unlocks the mutex. Consecutive calls will not throw an exception
        /// </summary>
        public void Unlock()
        {
            if (_mutex.CurrentCount == 0)
            {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Dispose of the <see cref="StatefulMutex"/> 
        /// </summary>
        public void Dispose()
        {
            _mutex.Dispose();
        }

    }
}
