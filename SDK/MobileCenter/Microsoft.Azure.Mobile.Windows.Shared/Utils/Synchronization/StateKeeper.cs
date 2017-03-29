using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
    public class StateKeeper
    {
        private State _currentState = new State();

        public void InvalidateState()
        {
            _currentState = _currentState.GetNextState();
        }

        public State GetStateSnapshot()
        {
            return _currentState;
        }

        public bool IsCurrent(State state)
        {
            return _currentState.Equals(state);
        }
    }
}
