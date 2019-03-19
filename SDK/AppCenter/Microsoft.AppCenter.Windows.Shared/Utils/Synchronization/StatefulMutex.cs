// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AppCenter.Utils.Synchronization
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

        /// <summary>
        /// Checks if the given state is current
        /// </summary>
        /// <param name="state">The state to test</param>
        /// <returns>True if the current state is current, false otherwise</returns>
        public bool IsCurrent(State state)
        {
            return _state.Equals(state);
        }

        public LockHolder GetLock()
        {
            _mutex.Wait();
            return new LockHolder(this);
        }

        public LockHolder GetLock(State state)
        {
            _mutex.Wait();
            if (IsCurrent(state))
            {
                return new LockHolder(this);
            }
            _mutex.Release();
            throw new StatefulMutexException("Cannot lock mutex with expired state");
        }

        public async Task<LockHolder> GetLockAsync()
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            return new LockHolder(this);
        }
        
        public async Task<LockHolder> GetLockAsync(State state)
        {
            await _mutex.WaitAsync().ConfigureAwait(false);
            if (IsCurrent(state))
            {
                return new LockHolder(this);
            }
            _mutex.Release();
            throw new StatefulMutexException("Cannot lock mutex with expired state");
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
            private readonly StatefulMutex _parent;

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
