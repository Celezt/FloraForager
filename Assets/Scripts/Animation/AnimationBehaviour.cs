using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBehaviour : MonoBehaviour
{
    private readonly static int VELOCITY_HASH = Animator.StringToHash("Velocity");

    public float Velocity
    {
        get => _velocity;
        set => _velocity = Mathf.Clamp01(value);
    }

    [SerializeField] private Animator _animator;

    private float _velocity;
    private float _oldVelocity;

    private void FixedUpdate()
    {
        if (_velocity != _oldVelocity)
            _animator.SetFloat(VELOCITY_HASH, Velocity);

        _oldVelocity = _velocity;
    }
}
