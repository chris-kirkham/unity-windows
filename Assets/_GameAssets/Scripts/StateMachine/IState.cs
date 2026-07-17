using UnityEngine;

namespace StateMachine
{
    public interface IState
    {
        public void EnterState();

        public void ExitState();
    }
}
