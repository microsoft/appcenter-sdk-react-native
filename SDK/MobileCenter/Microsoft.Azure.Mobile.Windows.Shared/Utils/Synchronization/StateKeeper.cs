using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
    /// <summary>
    /// Responsible for keeping track of a current <see cref="State"/>
    /// </summary>
    /// <seealso cref="StatefulMutex"/> 
    public class StateKeeper
    {
        private State _currentState = new State();

        /// <summary>
        /// Advances the current state
        /// </summary>
        public void InvalidateState()
        {
            _currentState = _currentState.GetNextState();
        }

        /// <summary>
        /// Gets the current state
        /// </summary>
        /// <returns>The current state</returns>
        public State GetStateSnapshot()
        {
            return _currentState;
        }

        /// <summary>
        /// Checks if the given state is current
        /// </summary>
        /// <param name="state">The state to test</param>
        /// <returns>True if the current state is current, false otherwise</returns>
        public bool IsCurrent(State state)
        {
            return _currentState.Equals(state);
        }
    }
}
