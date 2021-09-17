using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

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
    [SerializeField] private PivotMode _pivotMode;
    [SerializeField, ConditionalField(nameof(_pivotMode), false, PivotMode.Camera)] private Camera _camera;
    [SerializeField, ConditionalField(nameof(_pivotMode), false, PivotMode.Target)] private Transform _pivot;
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _drag = 8.0f;
    [SerializeField] private float _angularDrag = 5.0f;

    private enum PivotMode
    {
        None,
        Camera,
        Target,
    }

    private Vector3 _rawDirection;
    private Vector3 _rawVelocity;
    private Quaternion _rawRotation;
    private Vector3 _lookDirection;
    private Vector3 _velocity;
    private Quaternion _rotation;

    public void OnMovement(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _rawDirection = new Vector3(value.x, 0, value.y);
    }

    private void OnEnable()
    {
        _lookDirection = transform.forward;
    }

    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        _rawRotation = _rigidbody.rotation;

        Vector3 GetDirection(Vector3 pivotForward, Vector3 pivotRight)
        {
            pivotForward.y = 0f;
            pivotRight.y = 0f;

            Vector3 direction = (pivotForward * _rawDirection.z + pivotRight * _rawDirection.x).normalized;

            if (_rawDirection != Vector3.zero)
                _lookDirection = direction;

            return direction;
        }

        Vector3 pivotForward = Vector3.zero;
        Vector3 pivotRight = Vector3.zero;
        Vector3 currentDirection = Vector3.zero;
        switch (_pivotMode)
        {
            case PivotMode.Camera:
                if (_camera == null)
                    _camera = Camera.main;

                Transform cameraTransform = _camera.transform;
                pivotForward = cameraTransform.forward;
                pivotRight = cameraTransform.right;

                currentDirection = GetDirection(pivotForward, pivotRight);
                break;
            case PivotMode.Target:
                pivotForward = _pivot.forward;
                pivotRight = _pivot.right;

                currentDirection = GetDirection(pivotForward, pivotRight);
                break;
            default:
                if (_rawDirection != Vector3.zero)
                    _lookDirection = _rawDirection;

                currentDirection = _rawDirection;
                break;
        }

        void FixedMove()
        {
            _rawVelocity = currentDirection * _speed * fixedDeltaTime;
            _velocity = Vector3.Lerp(_velocity, _rawVelocity, _drag * fixedDeltaTime);
            _rigidbody.MovePosition(transform.position + _velocity);
        }

        void FixedRotate()
        {
            _rotation = Quaternion.Slerp(_rawRotation, Quaternion.LookRotation(_lookDirection, Vector3.up), _angularDrag * fixedDeltaTime);
            _rigidbody.MoveRotation(_rotation);
        }

        FixedMove();
        FixedRotate();
    }
}
