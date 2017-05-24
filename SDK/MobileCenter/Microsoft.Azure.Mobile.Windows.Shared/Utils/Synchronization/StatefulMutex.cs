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
        private State _state = new State();
        private volatile bool _isShutdown;
        private volatile int _waitingThreads;

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
        /// <returns>The new state</returns>
        public State InvalidateState()
        {
            _state = _state.GetNextState();
            return _state;
        }

        private void StartWait()
        {
            Interlocked.Increment(ref _waitingThreads);
            Interlocked.MemoryBarrier();
            if (_isShutdown)
            {
                Interlocked.Decrement(ref _waitingThreads);
                throw new StatefulMutexException("Trying to execute task after shutdown requested");
            }
        }

        private void StopWait()
        {
            Interlocked.Decrement(ref _waitingThreads);
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
            _isShutdown = true;
            Interlocked.MemoryBarrier();
            while (_waitingThreads > 0)
            {
                await _mutex.WaitAsync().ConfigureAwait(false);
                _mutex.Release();
            }
        }

        public async Task<bool> ShutdownAsync(TimeSpan timeout)
        {
            var result = true;
            var tokenSource = new CancellationTokenSource();
            var timeoutTask = Task.Delay(timeout, tokenSource.Token);
            _isShutdown = true;
            Interlocked.MemoryBarrier();
            while (result && _waitingThreads > 0)
            {
                var waitTask = _mutex.WaitAsync(tokenSource.Token);
                var _ = waitTask.ContinueWith(task => _mutex.Release(), TaskContinuationOptions.OnlyOnRanToCompletion);
                result = await Task.WhenAny(waitTask, timeoutTask).ConfigureAwait(false) != timeoutTask;
            }
            tokenSource.Cancel();
            return result;
        }
        
        public LockHolder GetLock()
        {
            StartWait();
            _mutex.Wait();
            StopWait();
            return new LockHolder(this);
        }

        public async Task<LockHolder> GetLockAsync()
        {
            StartWait();
            await _mutex.WaitAsync().ConfigureAwait(false);
            StopWait();
            return new LockHolder(this);
        }
        
        public async Task<LockHolder> GetLockAsync(State state)
        {
            StartWait();
            await _mutex.WaitAsync().ConfigureAwait(false);
            StopWait();
            if (!IsCurrent(state))
            {
                _mutex.Release();
                throw new StatefulMutexException("Cannot lock mutex with expired state");
            }
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
                _parent._mutex.Release();
            }
        }
    }
}
