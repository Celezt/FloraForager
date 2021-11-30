using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FSM;

public class LachBehaviour : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer _SkinnedRenderer;
    [SerializeField]
    private AnimationCurve _MouthIdle;
    [SerializeField]
    private AnimationCurve _MouthClose;
    [SerializeField]
    private bool _HasShrink;
    [SerializeField, ShowIf("_HasShrink")]
    private AnimationCurve _Shrink;

    private FSM_Machine _StateMachine;
    private float _InitialMouth;
    private float _InitialShrink;

    public readonly Idle IdleState = new Idle();
    public readonly Sleep SleepState = new Sleep();

    public SkinnedMeshRenderer SkinnedRenderer => _SkinnedRenderer;
    public FSM_Machine StateMachine => _StateMachine;
    public AnimationCurve MouthIdle => _MouthIdle;
    public AnimationCurve MouthClose => _MouthClose;
    public AnimationCurve Shrink => _Shrink;
    public bool HasShrink => _HasShrink;
    public float InitialMouth => _InitialMouth;
    public float InitialShrink => _InitialShrink;

    private void Start()
    {
        _InitialMouth = _SkinnedRenderer.GetBlendShapeWeight(0);
        _InitialShrink = _SkinnedRenderer.GetBlendShapeWeight(1);

        _StateMachine = new FSM_Machine();

        _StateMachine.AddState(IdleState);
        _StateMachine.AddState(SleepState);

        _StateMachine.InitializeStates(new LachContext { Lach = this });

        _StateMachine.TransitionTo(IdleState);
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

        private float _RandomSleep = 10.0f;

        public override void Init(object context)
        {
            _Context = context as LachContext;
        }

        public override void Enter()
        {
            _Context.Lach.SkinnedRenderer.SetBlendShapeWeight(0, _Context.Lach.InitialMouth);

            if (_Context.Lach.HasShrink)
                _Context.Lach.SkinnedRenderer.SetBlendShapeWeight(1, _Context.Lach.InitialShrink);

            _RandomSleep = Random.Range(0.0f, 10.0f);
        }

        public override void Update()
        {
            if (SleepSchedule.Instance.IsNightTime)
            {
                _RandomSleep -= Time.deltaTime;

                if (_RandomSleep <= 0.0f && _Context.Lach.StateMachine.TransitionTo(_Context.Lach.SleepState))
                    return;
            }

            _Context.Lach.SkinnedRenderer.SetBlendShapeWeight(0, Mathf.Max(0, _Context.Lach.InitialMouth + 100.0f * _Context.Lach.MouthIdle.Evaluate(Time.time)));
        }

        public override void Exit()
        {

        }
    }
    public class Sleep : State
    {
        private LachContext _Context;
        private float _InitialShrink;

        private float _Time;

        public override void Init(object context)
        {
            _Context = context as LachContext;
        }

        public override void Enter()
        {
            _InitialShrink = _Context.Lach.SkinnedRenderer.GetBlendShapeWeight(1);
            _Time = 0.0f;
        }

        public override void Update()
        {
            if (!SleepSchedule.Instance.IsNightTime)
            {
                if (_Context.Lach.StateMachine.TransitionTo(_Context.Lach.IdleState))
                    return;
            }

            _Time += Time.deltaTime;

            if (_Context.Lach.HasShrink)
                _Context.Lach.SkinnedRenderer.SetBlendShapeWeight(1, Mathf.SmoothStep(_InitialShrink, 100.0f, _Context.Lach.Shrink.Evaluate(_Time)));

            _Context.Lach.SkinnedRenderer.SetBlendShapeWeight(0, Mathf.SmoothStep(_Context.Lach.InitialMouth, 100.0f, _Context.Lach.MouthClose.Evaluate(_Time)));
        }

        public override void Exit()
        {

        }
    }
}
