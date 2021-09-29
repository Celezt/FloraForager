using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UseBehaviour : MonoBehaviour
{
    public PlayerAction Inputs => _playerAction;

    [SerializeField] private LayerMask _useLayers;

    private PlayerAction _playerAction;
    private PlayerInput _playerInput;
    private InputControlScheme _scheme;

    private Vector2 _screenPosition;
    private int _playerIndex;

    public void OnUse(InputAction.CallbackContext context)
    {
        void InteractAtCursor(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(_screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _useLayers))
            {
                IUsable tool = hit.transform.gameObject.GetComponent<IUsable>();

                if (tool == null)
                    return;
            }
        }

        InteractAtCursor(context);
    }

    public void OnCursor(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        _screenPosition = value;
    }

    public void ControlsChangedEvent(PlayerInput playerInput)
    {
        _playerIndex = playerInput.playerIndex;
        _scheme = playerInput.user.controlScheme.Value;
    }

    private void Start()
    {
        _playerAction = new PlayerAction();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        _playerAction.Enable();
        _playerAction.Ground.Use.started += OnUse;
        _playerAction.Ground.Use.performed += OnUse;
        _playerAction.Ground.Use.canceled += OnUse;
        _playerAction.Ground.Cursor.started += OnCursor;
        _playerAction.Ground.Cursor.performed += OnCursor;
        _playerAction.Ground.Cursor.canceled += OnCursor;
        _playerInput.controlsChangedEvent.AddListener(ControlsChangedEvent);
    }

    private void OnDisable()
    {
        _playerAction.Disable();
        _playerAction.Ground.Use.started -= OnUse;
        _playerAction.Ground.Use.performed -= OnUse;
        _playerAction.Ground.Use.canceled -= OnUse;
        _playerAction.Ground.Cursor.started -= OnCursor;
        _playerAction.Ground.Cursor.performed -= OnCursor;
        _playerAction.Ground.Cursor.canceled -= OnCursor;
        _playerInput.controlsChangedEvent.RemoveListener(ControlsChangedEvent);
    }
}
