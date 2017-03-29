namespace Microsoft.Azure.Mobile.Utils.Synchronization
{
    public class State
    {
        private readonly int _stateNum;

        public State() : this(0)
        {
        }

        private State(int stateNum)
        {
            _stateNum = stateNum;
        }

        public State GetNextState()
        {
            return new State(_stateNum + 1);
        }

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
