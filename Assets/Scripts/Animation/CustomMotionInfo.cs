using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public readonly struct CustomMotionInfo : IEquatable<CustomMotionInfo>
{
    public readonly AnimatorStateInfo stateInfo;
    public readonly Animator animator;
    public readonly AnimationBehaviour animationBehaviour;

    public bool Equals(CustomMotionInfo other) => stateInfo.Equals(other.stateInfo);

    public CustomMotionInfo(AnimatorStateInfo stateInfo, AnimationBehaviour animationBehaviour, Animator animator)
    {
        this.stateInfo = stateInfo;
        this.animator = animator;
        this.animationBehaviour = animationBehaviour;
    }
}
