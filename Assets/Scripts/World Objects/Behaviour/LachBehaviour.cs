using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class LachBehaviour : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer _SkinnedRenderer;
    [SerializeField]
    private AnimationCurve[] _PingPongBehaviour;

    private FSM_Machine _StateMachine;

    public readonly Idle IdleState = new Idle();
    public readonly Eat EatState = new Eat();
    public readonly Sleep SleepState = new Sleep();

    private void Start()
    {
        _StateMachine = new FSM_Machine();

        _StateMachine.AddState(IdleState);
        _StateMachine.AddState(EatState);
        _StateMachine.AddState(SleepState);

        _StateMachine.InitializeStates(new LachContext 
        { 
            LachBehaviour = this,
            SkinnedRenderer = _SkinnedRenderer, 
            StateMachine = _StateMachine 
        });

        _StateMachine.TransitionTo(IdleState);
    }

    private void Update()
    {
        _StateMachine.Update();
    }

    public class LachContext
    {
        public LachBehaviour LachBehaviour;
        public SkinnedMeshRenderer SkinnedRenderer;
        public FSM_Machine StateMachine;
    }

    public class Idle : State
    {
        private LachContext _Context;

        public override void Init(object context)
        {
            _Context = context as LachContext;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }
    }

    public class Eat : State
    {
        private LachContext _Context;

        public override void Init(object context)
        {
            _Context = context as LachContext;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }
    }

    public class Sleep : State
    {
        private LachContext _Context;

        public override void Init(object context)
        {
            _Context = context as LachContext;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {

        }

        public override void Exit()
        {

        }
    }
}
