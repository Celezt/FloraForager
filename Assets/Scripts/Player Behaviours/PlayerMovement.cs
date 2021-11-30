using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using IngameDebugConsole;
using Celezt.Time;
using Celezt.Mathematics;
using System.Linq;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Unprocessed input direction.
    /// </summary>
    public Vector3 RawInputValue => _activeInput.IsNullOrEmpty() ? _rawInputValue : Vector3.zero;
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
    public DurationCollection ActivaInput => _activeInput;
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
    /// Player's unlerped forward direction relative to the camera or the target. Never zero.
    /// </summary>
    public Vector3 RelativeForward
    {
        get => _relativeForward;
        set => _relativeForward = value != Vector3.zero ? value.normalized : _relativeForward;
    }
    /// <summary>
    /// Unlerped rotation
    /// </summary>
    public Quaternion RawRotation => Quaternion.LookRotation(_relativeForward, Vector3.up);
    /// <summary>
    /// Current rotation of the player.
    /// </summary>
    public Quaternion Rotation => _rotation;
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
    [SerializeField] private HumanoidAnimationBehaviour _animationBehaviour;
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
    private DurationCollection _activeInput = new DurationCollection();

    private Vector3 _slopeForward;
    private Vector3 _rawInputValue;
    private Vector3 _rawVelocity;
    private Vector3 _relativeForward;
    private Vector3 _velocity;
    private Vector3 _forward;
    private Quaternion _rotation;
    private float _groundAngle;
    private float _speed01;
    private bool _isRunning;
    private bool _isGrounded;
    private bool _isOnLedge;

    private PlayerInput _playerInput;

    /// <summary>
    /// Instantly set relative forward direction.
    /// </summary>
    /// <param name="forward">Direction.</param>
    public void SetDirection(Vector2 forward)
    {
        if (forward == Vector2.zero)
            return;

        _relativeForward = forward.xz().normalized;
        _rotation = RawRotation;
        _rigidbody.MoveRotation(_rotation);
    }
    public void SetRunMultiplier(float multiplier) => _runningSpeedMultiplier = multiplier;
    public void SetDrag(float drag) => _drag = drag;
    public void SetAngularDrag(float angularDrag) => _angularDrag = angularDrag;
    public void SetGroundCheckDistance(float distance) => _groundCheckDistance = distance;
    public void SetMaxSlopeAngle(float degree) => _maxSlopeAngle = degree;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        _rawInputValue = new Vector3(value.x, 0, value.y);
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
        DebugLogConsole.AddCommandInstance("player.direction", "Sets player's relative forward direction value", nameof(SetDirection), this);
        DebugLogConsole.AddCommandInstance("player.speed", "Sets player's base speed value", nameof(SetBaseSpeed), this);
        DebugLogConsole.AddCommandInstance("player.run", "Sets player's run multiplier", nameof(SetRunMultiplier), this);
        DebugLogConsole.AddCommandInstance("player.drag", "Sets player's drag value", nameof(SetDrag), this);
        DebugLogConsole.AddCommandInstance("player.ground_check_distance", "Sets player's ground check distance if on the ground", nameof(SetGroundCheckDistance), this);
        DebugLogConsole.AddCommandInstance("player.max_slope_angle", "Sets player's max slope angle that are possible to move over", nameof(SetMaxSlopeAngle), this);

        _rigidbody.freezeRotation = true;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, -Vector3.up, out RaycastHit hit, 10f, _groundLayer))
            transform.position = hit.point;
    }

    private void FixedUpdate()
    {
        Vector3 position = transform.position;
        Vector3 inputValue = _activeInput.IsNullOrEmpty() ? _rawInputValue : Vector3.zero;    // If input is enabled.
        float fixedDeltaTime = Time.fixedDeltaTime;

        void Movement()
        {
            void GetDirection(Vector3 pivotForward, Vector3 pivotRight)
            {
                pivotForward.y = 0f;
                pivotRight.y = 0f;

                _forward = (pivotForward * inputValue.z + pivotRight * inputValue.x).normalized;

                if (inputValue != Vector3.zero)
                    _relativeForward = _forward;
            }

            switch (_pivotMode)
            {
                case PivotMode.Camera:
                    if (_camera == null)
                        _camera = Camera.main;

                    Transform cameraTransform = _camera.transform;
                    GetDirection(cameraTransform.forward, cameraTransform.right);
                    break;
                case PivotMode.Target:
                    GetDirection(_pivot.forward, _pivot.right);
                    break;
                default:
                    if (inputValue != Vector3.zero)
                        _relativeForward = inputValue;

                    _forward = inputValue;
                    break;
            }

            void FixedMove()
            {
                _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y > float.Epsilon ? 0 : _rigidbody.velocity.y, 0);

                if (_groundAngle >= _maxSlopeAngle)
                    return;

                _rawVelocity = _slopeForward * CurrentSpeed;
                _velocity = Vector3.Lerp(_velocity, _rawVelocity, _drag * fixedDeltaTime);

                if (Physics.Raycast(position + Vector3.up * 0.3f, _velocity, 0.5f))
                    _velocity = new Vector3(_velocity.x, 0, _velocity.z);

                _rigidbody.AddForce(_velocity, ForceMode.VelocityChange);
            }

            void FixedRotate()
            {
                _rotation = Quaternion.Slerp(_rigidbody.rotation, RawRotation, _angularDrag * fixedDeltaTime);
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
                _isGrounded = Physics.Raycast(position + Vector3.up * 0.1f, -Vector3.up, out hit, _groundCheckDistance, _groundLayer);
            }

            void ForwardSlope()
            {
                if (!_isGrounded)
                {
                    _slopeForward = Vector3.zero;
                    return;
                }

                _slopeForward = Vector3.Cross(Vector3.Cross(Vector3.up, _forward), hit.normal).normalized;
            };

            void GroundAngle()
            {
                _groundAngle = Vector3.Dot(_slopeForward, Vector3.down) <= 0.0f ? Vector3.Angle(Vector3.up, hit.normal) : 0.0f;
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

        void Animation()
        {
            if (_animationBehaviour == null)
                return;

            _speed01 = Mathf.Lerp(_speed01, _isRunning ? 
                  cmath.Map(inputValue.magnitude, new MinMaxFloat(0, 1), new MinMaxFloat(0.0f, 1)) 
                : cmath.Map(inputValue.magnitude, new MinMaxFloat(0, 1), new MinMaxFloat(0, 0.3f)), _drag * fixedDeltaTime);
            _animationBehaviour.WalkSpeed = _speed01;
        }

        SlopeMovement(); 
        Movement();
        PhysicMaterialToUse();
        Animation();
    }

    private void SetBaseSpeed(float speed) => _baseSpeed = speed;
}
