using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using IngameDebugConsole;
using Celezt.Time;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Unprocessed input direction.
    /// </summary>
    public Vector3 RawDirection => _rawDirection;
    /// <summary>
    /// Forward direction accounting for ground normal.
    /// </summary>
    public Vector3 SlopeForward => _slopeForward;
    /// <summary>
    /// Current player speed including when the player is running.
    /// </summary>
    public float CurrentSpeed
    {
        get
        {
            float totalMultiplier = 1;

            _speedMultipliers.ForEach(x => totalMultiplier *= x.Value);

            return _isRunning ? _baseSpeed * totalMultiplier * _runningSpeedMultiplier : _baseSpeed * totalMultiplier;
        }
    }
    /// <summary>
    /// Base speed of the player.
    /// </summary>
    public float BaseSpeed => _baseSpeed;

    public DurationCollection<float> SpeedMultipliers => _speedMultipliers;
    public DurationCollection<bool> ActivaInput => _activeInput;

    /// <summary>
    /// Running multiplier.
    /// </summary>
    public float RunningSpeedMultiplier
    {
        get => _runningSpeedMultiplier;
        set => _runningSpeedMultiplier = value;
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
    /// <summary>
    /// If currently running.
    /// </summary>
    public bool IsRunning => _isRunning;
    /// <summary>
    /// If currently touching the ground.
    /// </summary>
    public bool IsGrounded => _isGrounded;

    /// <summary>
    /// Callback for when player's running state has changed (current speed, ).
    /// </summary>
    public event Action<float, bool> OnPlayerRunCallback = delegate { };

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    [SerializeField] private PivotMode _pivotMode;
    [SerializeField, ConditionalField(nameof(_pivotMode), false, PivotMode.Camera)] private Camera _camera;
    [SerializeField, ConditionalField(nameof(_pivotMode), false, PivotMode.Target)] private Transform _pivot;
    [SerializeField] private float _baseSpeed = 6.0f;
    [SerializeField, Min(0)] private float _runningSpeedMultiplier = 2.0f;
    [SerializeField] private float _drag = 8.0f;
    [SerializeField] private float _angularDrag = 5.0f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 1.5f;
    [SerializeField, Tooltip("In degrees."), Range(0, 180)] private float _maxSlopeAngle = 45.0f;
    [SerializeField] private PhysicMaterial _groundPhysicMaterial;
    [SerializeField] private PhysicMaterial _fallPhysicMaterial;

    private enum PivotMode
    {
        None,
        Camera,
        Target,
    }

    private DurationCollection<float> _speedMultipliers = new DurationCollection<float>();
    private DurationCollection<bool> _activeInput = new DurationCollection<bool>();

    private Vector3 _slopeForward;
    private Vector3 _rawDirection;
    private Vector3 _rawVelocity;
    private Vector3 _relativeForward;
    private Vector3 _velocity;
    private Vector3 _forward;
    private Quaternion _rawRotation;
    private Quaternion _rotation;
    private float _groundAngle;
    private bool _isRunning;
    private bool _isGrounded;
    private bool _isOnLedge;

    private PlayerInput _playerInput;

    public void SetRunMultiplier(float multiplier) => _runningSpeedMultiplier = multiplier;
    public void SetDrag(float drag) => _drag = drag;
    public void SetAngularDrag(float angularDrag) => _angularDrag = angularDrag;
    public void SetGroundCheckDistance(float distance) => _groundCheckDistance = distance;
    public void SetMaxSlopeAngle(float degree) => _maxSlopeAngle = degree;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _rawDirection = new Vector3(value.x, 0, value.y);
        _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);

        //if (context.canceled)
        //    _playerInput.actions["Run"].Reset();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        bool oldIsRunning = _isRunning;

        _isRunning = value > 0.5f;

        if (oldIsRunning != _isRunning)
            OnPlayerRunCallback.Invoke(CurrentSpeed, _isRunning);
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _relativeForward = transform.forward;
    }

    private void Start()
    {
        DebugLogConsole.AddCommandInstance("player.speed", "Sets player's base speed value", nameof(SetBaseSpeed), this);
        DebugLogConsole.AddCommandInstance("player.run", "Sets player's run multiplier", nameof(SetRunMultiplier), this);
        DebugLogConsole.AddCommandInstance("player.drag", "Sets player's drag value", nameof(SetDrag), this);
        DebugLogConsole.AddCommandInstance("player.ground_check_distance", "Sets player's ground check distance if on the ground", nameof(SetGroundCheckDistance), this);
        DebugLogConsole.AddCommandInstance("player.max_slope_angle", "Sets player's max slope angle that are possible to move over", nameof(SetMaxSlopeAngle), this);
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;

        void Movement()
        {
            float fixedDeltaTime = Time.fixedDeltaTime;
            _rawRotation = _rigidbody.rotation;

            void GetDirection(Vector3 pivotForward, Vector3 pivotRight)
            {
                pivotForward.y = 0f;
                pivotRight.y = 0f;

                _forward = (pivotForward * _rawDirection.z + pivotRight * _rawDirection.x).normalized;

                if (_rawDirection != Vector3.zero)
                    _relativeForward = _forward;
            }

            Vector3 pivotForward = Vector3.zero;
            Vector3 pivotRight = Vector3.zero;
            switch (_pivotMode)
            {
                case PivotMode.Camera:
                    if (_camera == null)
                        _camera = Camera.main;

                    Transform cameraTransform = _camera.transform;
                    pivotForward = cameraTransform.forward;
                    pivotRight = cameraTransform.right;

                    GetDirection(pivotForward, pivotRight);
                    break;
                case PivotMode.Target:
                    pivotForward = _pivot.forward;
                    pivotRight = _pivot.right;

                    GetDirection(pivotForward, pivotRight);
                    break;
                default:
                    if (_rawDirection != Vector3.zero)
                        _relativeForward = _rawDirection;

                    _forward = _rawDirection;
                    break;
            }

            void FixedMove()
            {
                if (_groundAngle >= _maxSlopeAngle)
                    return;

                _rawVelocity = _slopeForward * CurrentSpeed * fixedDeltaTime;
                _velocity = Vector3.Lerp(_velocity, _rawVelocity, _drag * fixedDeltaTime);
                _rigidbody.MovePosition(position + _velocity);
            }

            void FixedRotate()
            {
                _rotation = Quaternion.Slerp(_rawRotation, Quaternion.LookRotation(_relativeForward, Vector3.up), _angularDrag * fixedDeltaTime);
                _rigidbody.MoveRotation(_rotation);
            }

            FixedMove();
            FixedRotate();
        }

        void SlopeMovement()
        {
            RaycastHit hit = new RaycastHit();

            void CheckGround()
            {
                _isGrounded = Physics.Raycast(position, -Vector3.up, out hit, _groundCheckDistance, _groundLayer);
            }

            void ForwardSlope()
            {
                if (!_isGrounded)
                {
                    _slopeForward = _forward;
                    return;
                }

                _slopeForward = Vector3.Cross(Vector3.Cross(Vector3.up, _forward), hit.normal).normalized;
            };

            void GroundAngle()
            {
                _groundAngle = Vector3.Angle(hit.normal, _forward) - 90.0f;
            }

            void DrawDebugLines()
            {
#if UNITY_EDITOR
                if (DebugManager.DebugMode)
                {
                    Debug.DrawLine(position, position + _slopeForward * _groundCheckDistance * 2, Color.blue);
                    Debug.DrawLine(position, position - Vector3.up * _groundCheckDistance, Color.green);
                }
#endif
            }

            CheckGround();
            ForwardSlope();
            GroundAngle();
            DrawDebugLines();
        }

        void PhysicMaterialToUse()
        {
            _collider.material = _isGrounded ? _groundPhysicMaterial : _fallPhysicMaterial;
        }

        Movement();
        SlopeMovement();
        PhysicMaterialToUse();
    }

    private void SetBaseSpeed(float speed) => _baseSpeed = speed;
}
