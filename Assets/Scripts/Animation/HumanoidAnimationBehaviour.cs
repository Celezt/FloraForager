using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationBehaviour : MonoBehaviour
{
    private static readonly int VELOCITY_HASH = Animator.StringToHash("Velocity");
    private static readonly int CUSTOM_INDEX_HASH = Animator.StringToHash("CustomIndex");
    private static readonly int CUSTOM_MOTION_SPEED_HASH = Animator.StringToHash("CustomMotionSpeed");
    private static readonly int CUSTOM_TRIGGER_HASH = Animator.StringToHash("CustomTrigger");
    private static readonly int IS_CUSTOM_LOOP_HASH = Animator.StringToHash("IsCustomLoop");
    private static readonly int IS_CUSTOM_FORCE_CANCEL_TRIGGER_HASH = Animator.StringToHash("CustomForceCancelTrigger");
    private static readonly int IS_CUSTOM_BLEND_CANCEL_TRIGGER_HASH = Animator.StringToHash("CustomBlendCancelTrigger");

    /// <summary>
    /// Transform to hold items.
    /// </summary>
    public Transform HoldTransform => _holdTransform;
    /// <summary>
    /// Speed between 0 and 1.
    /// </summary>
    public float WalkSpeed
    {
        get => _walkSpeed;
        set => _walkSpeed = Mathf.Clamp01(value);
    }
    public AnimationClip CurrentCustomClip => _currentCustomClip;
    /// <summary>
    /// If currently in a loop.
    /// </summary>
    public bool IsCustomMotionLooping => _isCustomMotionLooping;
    /// <summary>
    /// If currently running a custom animation.
    /// </summary>
    public bool IsCustomMotionRunning => _isCustomMotionRunning;
     
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _holdTransform;
    [SerializeField] private Vector3 _target;

    private AnimatorOverrideController _animatorOverrideController;
    private Queue<System.Action<CustomMotionInfo>> _exitCustomMotionQueue = new Queue<System.Action<CustomMotionInfo>>();
    private System.Action<CustomMotionInfo> _enterCustomMotion;
    private Transform _headTransform;
    private AnimationClip _currentCustomClip;

    private Quaternion _rotationHeadOffset;
    private float _walkSpeed;
    private float _oldVelocity;
    private int _customIndex;
    private bool _isCustomMotionRunning;
    private bool _isCustomMotionLooping;

    /// <summary>
    /// Manually cancel custom motion loop. Will run until the current loop is done.
    /// </summary>
    public void CancelLoop() => _animator.SetBool(IS_CUSTOM_LOOP_HASH, false);

    /// <summary>
    /// Instant cancel the current custom motion.
    /// </summary>
    public void ForceCancelCustomMotion() => _animator.SetTrigger(IS_CUSTOM_FORCE_CANCEL_TRIGGER_HASH);
    /// <summary>
    /// Cancel and blend the current custom motion.
    /// </summary>
    public void BlendCancelCustomMotion() => _animator.SetTrigger(IS_CUSTOM_BLEND_CANCEL_TRIGGER_HASH);

    /// <summary>
    /// Raise a new custom animation.
    /// </summary>
    /// <param name="motion">Custom animation.</param>
    /// <param name="loop">Loop animation until manually canceled or another custom motion is activated.</param>
    public void CustomMotionRaise(AnimationClip clip, float speedMultiplier = 1, bool loop = false, System.Action<CustomMotionInfo> enterCallback = null, System.Action<CustomMotionInfo> exitCallback = null)
    {
        if (clip == null)
            return;

        _isCustomMotionRunning = true;

        if (_customIndex > 0)
            _animatorOverrideController["EmptyCustomMotion1"] = clip;
        else
            _animatorOverrideController["EmptyCustomMotion0"] = clip;

        _currentCustomClip = clip;
        _animator.SetLayerWeight(1, 0);
        _animator.SetBool(IS_CUSTOM_LOOP_HASH, _isCustomMotionLooping = loop);
        _animator.SetInteger(CUSTOM_INDEX_HASH, _customIndex);
        _animator.SetFloat(CUSTOM_MOTION_SPEED_HASH, speedMultiplier);
        _animator.SetTrigger(CUSTOM_TRIGGER_HASH);
        _enterCustomMotion = enterCallback;
        _exitCustomMotionQueue.Enqueue(exitCallback);
        _customIndex = (_customIndex + 1) % 2;
    }

    public void InternalCustomMotionEnterRaise(CustomMotionInfo info)
    {
        _enterCustomMotion?.Invoke(info);
        _enterCustomMotion = null;
    }

    public void InternalCustomMotionExitRaise(CustomMotionInfo info)
    {
        if (_exitCustomMotionQueue.Count > 0)
            _exitCustomMotionQueue.Dequeue()?.Invoke(info);

        _isCustomMotionRunning = (_exitCustomMotionQueue.Count > 0);

        if (!_isCustomMotionRunning)
        {
            _animator.SetLayerWeight(1, 1);
            _currentCustomClip = null;
        }
    }

    private void Start()
    {
        _headTransform = _animator.GetBoneTransform(HumanBodyBones.Neck);
        _rotationHeadOffset = _headTransform.localRotation;

        _animatorOverrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _animatorOverrideController;
    }

    private void FixedUpdate()
    {
        if (_walkSpeed != _oldVelocity)
            _animator.SetFloat(VELOCITY_HASH, WalkSpeed);

        _oldVelocity = _walkSpeed;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //Quaternion rotation = Quaternion.LookRotation(_target - _headTransform.position);
        //_animator.SetBoneLocalRotation(HumanBodyBones.Neck, _rotationHeadOffset * Quaternion.Inverse(_headTransform.rotation) * rotation * Quaternion.Euler(_rotationHeadOffset.eulerAngles.x * 0.75f, 0, 0));
    }
}
