using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CustomMotionBehaviour : StateMachineBehaviour
{
    private HumanoidAnimationBehaviour _animationBehaviour;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animationBehaviour == null)
            _animationBehaviour = animator.GetComponentInParent<HumanoidAnimationBehaviour>();

        _animationBehaviour.InternalCustomMotionEnterRaise(new CustomMotionInfo(stateInfo, _animationBehaviour, animator));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animationBehaviour.InternalCustomMotionExitRaise(new CustomMotionInfo(stateInfo, _animationBehaviour, animator));
    }
}
