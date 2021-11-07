using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimationBehaviour : MonoBehaviour
{
    private readonly static int VELOCITY_HASH = Animator.StringToHash("Velocity");
    public float Velocity
    {
        get => _velocity;
        set => _velocity = Mathf.Clamp01(value);
    }

    [SerializeField] private Animator _animator;
    [SerializeField] private Vector3 _target;

    private Transform _headTransform;
    private Quaternion _rotationHeadOffset;
    private float _velocity;
    private float _oldVelocity;

    private void Start()
    {
        _headTransform = _animator.GetBoneTransform(HumanBodyBones.Neck);
        _rotationHeadOffset = _headTransform.localRotation;
    }

    private void FixedUpdate()
    {
        if (_velocity != _oldVelocity)
            _animator.SetFloat(VELOCITY_HASH, Velocity);

        _oldVelocity = _velocity;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        Quaternion rotation = Quaternion.LookRotation(_target - _headTransform.position);
        _animator.SetBoneLocalRotation(HumanBodyBones.Neck, _rotationHeadOffset * Quaternion.Inverse(_headTransform.rotation) * rotation * Quaternion.Euler(_rotationHeadOffset.eulerAngles.x * 0.75f, 0, 0));
    }
}
