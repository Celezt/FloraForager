using UnityEngine;

namespace FSM
{
    public abstract class State
    {
        public abstract void Init(object context);
        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
    }
}
