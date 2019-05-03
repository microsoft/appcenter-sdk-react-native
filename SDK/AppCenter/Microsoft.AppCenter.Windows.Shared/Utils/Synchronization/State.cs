// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.AppCenter.Utils.Synchronization
{
    /// <summary>
    /// Represents a particular state
    /// </summary>
    /// <seealso cref="StateKeeper"/>
    /// <seealso cref="StatefulMutex"/> 
    public class State
    {
        private readonly int _stateNum;
        
        /// <summary>
        /// Creates a new state
        /// </summary>
        public State() : this(0)
        {
        }

        private State(int stateNum)
        {
            _stateNum = stateNum;
        }

        /// <summary>
        /// Creates a new state that follows the current state
        /// </summary>
        /// <returns></returns>
        public State GetNextState()
        {
            return new State(_stateNum + 1);
        }

        /// <summary>
        /// Compares states by state number
        /// </summary>
        /// <param name="obj">The object to compare to</param>
        /// <returns>True if they have the same state number, false otherwise</returns>
        public override bool Equals(object obj)
        {
            var otherState = obj as State;
            return otherState?._stateNum == _stateNum;
        }

        public override int GetHashCode()
        {
            return _stateNum.GetHashCode();
        }
    }
}
