using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CustomMotionBehaviour : StateMachineBehaviour
{
    private AnimationBehaviour _animationBehaviour;
    private Coroutine exitCoroutine;
    public int i;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_animationBehaviour == null)
            _animationBehaviour = animator.GetComponentInParent<AnimationBehaviour>();

        _animationBehaviour.ExitCustomMotionQueue.Enqueue(exitCallback);
        Debug.Log("v: " + i);
        _animationBehaviour.InternalCustomMotionEnterRaise(new CustomMotionInfo(stateInfo, _animationBehaviour, animator));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (exitCoroutine != null)
            _animationBehaviour.StopCoroutine(exitCoroutine);

        exitCoroutine = _animationBehaviour.StartCoroutine(Exit(new CustomMotionInfo(stateInfo, _animationBehaviour, animator)));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animationBehaviour.InternalCustomMotionExitRaise(new CustomMotionInfo(stateInfo, _animationBehaviour, animator));

        if (exitCoroutine != null)
            _animationBehaviour.StopCoroutine(exitCoroutine);

    }

    private IEnumerator Exit(CustomMotionInfo info)
    {
        yield return new WaitForSeconds(Time.deltaTime * 5f);
        Debug.Log("ööö");
        _animationBehaviour.InternalCustomMotionExitRaise(info);
    }
}
