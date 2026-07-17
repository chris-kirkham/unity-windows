using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public class StateMachine
    {
        [SerializeField] private List<IState> states;

        private IState currentState;

        public void OnEnable()
        {
            if(states.Count > 0)
            {
                SetState(states[0]);
            }
        }

        private void SetState(IState state)
        {
            state.EnterState();
            currentState = state;
        }
    }
}