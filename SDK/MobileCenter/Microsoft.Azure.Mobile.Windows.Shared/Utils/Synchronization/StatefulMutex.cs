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
        private const string ShutdownExceptionMessage = "Trying to execute task after shutdown requested";
        private const string StateExceptionMessage = "Cannot lock mutex with expired state";

        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);
        private State _state = new State();
        private volatile bool _isShutdown;

        /// <summary>
        /// Gets the current state
        /// </summary>
        /// <returns>The current state</returns>
        public State State
        {
            get { return _state; }
        }

        /// <summary>
        /// Advances the current state
        /// </summary>
        public void InvalidateState()
        {
            _state = _state.GetNextState();
        }

        /// <summary>
        /// Checks if the given state is current
        /// </summary>
        /// <param name="state">The state to test</param>
        /// <returns>True if the current state is current, false otherwise</returns>
        public bool IsCurrent(State state)
        {
            return _state.Equals(state);
        }

        public async Task ShutdownAsync()
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            _isShutdown = true;
            _mutex.Release();
        }

        public async Task<bool> ShutdownAsync(TimeSpan timeout)
        {
            var result = await _mutex.WaitAsync(timeout).ConfigureAwait(false);
            _isShutdown = true;
            _mutex.Release();
            return result;
        }

        /// <summary>
        /// Locks the mutex and does not verify any state
        /// </summary>
        /// <seealso cref="LockAsync"/>
        public void Lock()
        {
            _mutex.Wait();
            if (_isShutdown)
            {
                throw new StatefulMutexException(ShutdownExceptionMessage);
            }
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
            if (_isShutdown)
            {
                throw new StatefulMutexException(ShutdownExceptionMessage);
            }
            if (!IsCurrent(stateSnapshot))
            {
                throw new StatefulMutexException(StateExceptionMessage);
            }
        }

        /// <summary>
        /// Asynchronously locks the mutex and does not verify any state
        /// </summary>
        /// <seealso cref="Lock"/>
        public async Task LockAsync()
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            if (_isShutdown)
            {
                throw new StatefulMutexException(ShutdownExceptionMessage);
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
            await _mutex.WaitAsync().ConfigureAwait(false);
            if (_isShutdown)
            {
                throw new StatefulMutexException(ShutdownExceptionMessage);
            }
            if (!IsCurrent(stateSnapshot))
            {
                Unlock();
                throw new StatefulMutexException(StateExceptionMessage);
            }
        }

        /// <summary>
        /// Unlocks the mutex. Consecutive calls will not throw an exception
        /// </summary>
        public void Unlock()
        {
            if (_mutex.CurrentCount != 0)
            {
                throw new StatefulMutexException("Trying to unlock not locked mutex");
            }
            _mutex.Release();
        }

        public LockHolder GetLock()
        {
            Lock();
            return new LockHolder(this);
        }

        public async Task<LockHolder> GetLockAsync()
        {
            await LockAsync().ConfigureAwait(false);
            return new LockHolder(this);
        }
        
        public async Task<LockHolder> GetLockAsync(State state)
        {
            await LockAsync(state).ConfigureAwait(false);
            return new LockHolder(this);
        }

        /// <summary>
        /// Dispose of the <see cref="StatefulMutex"/> 
        /// </summary>
        public void Dispose()
        {
            _mutex.Dispose();
        }

        public class LockHolder : IDisposable
        {
            private StatefulMutex _parent;

            internal LockHolder(StatefulMutex parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
                _parent.Unlock();
            }
        }
    }
}
