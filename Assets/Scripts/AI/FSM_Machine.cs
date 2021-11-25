using System.Collections.Generic;

namespace FSM
{
    public class FSM_Machine
    {
        private State _CurrentState = null;

        private readonly List<State> States;

        public FSM_Machine()
        {
            States = new List<State>();
        }

        public void Update()
        {
            _CurrentState?.Update();
        }

        public void InitializeStates(object context)
        {
            States.ForEach(s => s.Init(context));
        }

        public void AddState(State state)
        {
            States.Add(state);
        }

        public bool TransitionTo(State state)
        {
            if (state == _CurrentState || state == null)
                return false;

            _CurrentState?.Exit();
            _CurrentState = state;
            _CurrentState.Enter();

            return true;
        }
    }
}
