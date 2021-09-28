using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using IngameDebugConsole;

public class PlayerMovement : MonoBehaviour
{
    public PlayerAction Inputs => _inputs;

    /// <summary>
    /// Unprocessed input direction.
    /// </summary>
    public Vector3 RawDirection => _rawDirection;
    /// <summary>
    /// Forward direction accounting for ground normal.
    /// </summary>
    public Vector3 SlopeForward => _slopeForward;
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
    /// <summary>
    /// If currently running.
    /// </summary>
    public bool IsRunning => _isRunning;
    /// <summary>
    /// If currently touching the ground.
    /// </summary>
    public bool IsGrounded => _isGrounded;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PivotMode _pivotMode;
    [SerializeField, ConditionalField(nameof(_pivotMode), false, PivotMode.Camera)] private Camera _camera;
    [SerializeField, ConditionalField(nameof(_pivotMode), false, PivotMode.Target)] private Transform _pivot;
    [SerializeField] private float _speed = 6.0f;
    [SerializeField, Min(0)] private float _runningSpeedMultiplier = 2.0f;
    [SerializeField] private float _drag = 8.0f;
    [SerializeField] private float _angularDrag = 5.0f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundCheckDistance = 1.5f;
    [SerializeField, Tooltip("In degrees."), Range(0, 180)] private float _maxSlopeAngle = 45.0f;

    private enum PivotMode
    {
        None,
        Camera,
        Target,
    }

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
    private PlayerAction _inputs;

    public void SetSpeed(float speed) => _speed = speed;
    public void SetRunMultiplier(float multiplier) => _runningSpeedMultiplier = multiplier;
    public void SetDrag(float drag) => _drag = drag;
    public void SetAngularDrag(float angularDrag) => _angularDrag = angularDrag;
    public void SetGroundCheckDistance(float distance) => _groundCheckDistance = distance;
    public void SetMaxSlopeAngle(float degree) => _maxSlopeAngle = degree;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _rawDirection = new Vector3(value.x, 0, value.y);

        if (context.canceled)
        {
            _inputs.Ground.Run.Reset();
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        _isRunning = value > 0.5f;
    }

    private void Awake()
    {
        _inputs = new PlayerAction();
    }

    private void Start()
    {
        DebugLogConsole.AddCommandInstance("player_speed", "Set player's speed value", nameof(SetSpeed), this);
        DebugLogConsole.AddCommandInstance("player_run", "Set player's run multiplier", nameof(SetRunMultiplier), this);
        DebugLogConsole.AddCommandInstance("player_drag", "Set player's drag value", nameof(SetDrag), this);
        DebugLogConsole.AddCommandInstance("player_ground_check_distance", "Set player's ground check distance if on the ground", nameof(SetGroundCheckDistance), this);
        DebugLogConsole.AddCommandInstance("player_max_slope_angle", "Set player's max slope angle that are possible to move over", nameof(SetMaxSlopeAngle), this);
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Ground.Move.started += OnMove;
        _inputs.Ground.Move.performed += OnMove;
        _inputs.Ground.Move.canceled += OnMove;
        _inputs.Ground.Run.started += OnRun;
        _inputs.Ground.Run.performed += OnRun;
        _inputs.Ground.Run.canceled += OnRun;

        _relativeForward = transform.forward;
    }

    private void OnDisable()
    {
        _inputs.Disable();
        _inputs.Ground.Move.started -= OnMove;
        _inputs.Ground.Move.performed -= OnMove;
        _inputs.Ground.Move.canceled -= OnMove;
        _inputs.Ground.Run.started -= OnRun;
        _inputs.Ground.Run.performed -= OnRun;
        _inputs.Ground.Run.canceled -= OnRun;
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;

        Movement(position);
        SlopeMovement(position);
    }

    private void Movement(Vector3 position)
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

            _rawVelocity = _slopeForward * ((_isRunning) ? _speed * _runningSpeedMultiplier : _speed) * fixedDeltaTime;
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

    private void SlopeMovement(Vector3 position)
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
}
