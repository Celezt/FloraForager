using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Unprocessed input direction.
    /// </summary>
    public Vector3 RawDirection => _rawDirection;
    /// <summary>
    /// Current speed of the player.
    /// </summary>
    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }
    /// <summary>
    /// Unlerped velocity.
    /// </summary>
    public Vector3 RawVelocity => _rawVelocity;
    /// <summary>
    /// Current velocity of the player.
    /// </summary>
    public Vector3 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }
    /// <summary>
    /// Unslerped rotation.
    /// </summary>
    public Quaternion RawRotation => _rawRotation;
    /// <summary>
    /// Current rotation of the player.
    /// </summary>
    public Quaternion Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _drag = 8.0f;
    [SerializeField] private float _angularDrag = 5.0f;

    private Vector3 _forward;
    private Vector3 _rawDirection;
    private Vector3 _rawVelocity;
    private Quaternion _rawRotation;
    private Vector3 _velocity;
    private Quaternion _rotation;

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _rawDirection = new Vector3(value.x, 0, value.y);

        if (_rawDirection != Vector3.zero)
            _forward = _rawDirection;
    }

    private void OnEnable()
    {
        _forward = transform.forward;
    }

    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        Vector3 forward = _forward;
        _rawRotation = _rigidbody.rotation;
        
        void FixedMove()
        {
            _rawVelocity = _rawDirection * _speed * fixedDeltaTime;
            _velocity = Vector3.Lerp(_velocity, _rawVelocity, _drag * fixedDeltaTime);
            _rigidbody.MovePosition(transform.position + _velocity);
        }

        void FixedRotate()
        {
            _rotation = Quaternion.Slerp(_rawRotation, Quaternion.LookRotation(forward, Vector3.up), _angularDrag * fixedDeltaTime);
            _rigidbody.MoveRotation(_rotation);
        }

        FixedMove();
        FixedRotate();
    }
}
