using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class LachBehaviour : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer _SkinnedRenderer;
    [SerializeField]
    private AnimationCurve _MouthBehaviour;

    private FSM_Machine _StateMachine;
    private float _InitialMouth;

    public readonly Idle IdleState = new Idle();
    public readonly Eat EatState = new Eat();
    public readonly Sleep SleepState = new Sleep();

    public SkinnedMeshRenderer SkinnedRenderer => _SkinnedRenderer;
    public AnimationCurve MouthBehaviour => _MouthBehaviour;
    public FSM_Machine StateMachine => _StateMachine;
    public float InitialMouth => _InitialMouth;

    private void Start()
    {
        _StateMachine = new FSM_Machine();

        _StateMachine.AddState(IdleState);
        _StateMachine.AddState(EatState);
        _StateMachine.AddState(SleepState);

        _StateMachine.InitializeStates(new LachContext { Lach = this });

        _StateMachine.TransitionTo(IdleState);

        _InitialMouth = _SkinnedRenderer.GetBlendShapeWeight(0);
    }

    private void Update()
    {
        _StateMachine.Update();
    }

    public class LachContext
    {
        public LachBehaviour Lach;
    }

    public class Idle : State
    {
        private LachContext _Context;
        private float _InitialMouth;

        public override void Init(object context)
        {
            _Context = context as LachContext;
            _InitialMouth = _Context.Lach.InitialMouth;
        }

        public override void Enter()
        {

        }

        public override void Update()
        {
            _Context.Lach.SkinnedRenderer.SetBlendShapeWeight(0, _InitialMouth + 100.0f * _Context.Lach.MouthBehaviour.Evaluate(Time.time));

            if (SleepSchedule.Instance.IsNightTime)
                _Context.Lach.StateMachine.TransitionTo(_Context.Lach.SleepState);
        }

        public override void Exit()
        {

        }
    }

    public class Eat : State
    {
        private LachContext _Context;
        private float _InitialMouth;

        public override void Init(object context)
        {
            _Context = context as LachContext;
            _InitialMouth = _Context.Lach.InitialMouth;
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

        private float _InitialMouth;

        public override void Init(object context)
        {
            _Context = context as LachContext;
            _InitialMouth = _Context.Lach.SkinnedRenderer.GetBlendShapeWeight(0);
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
