using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UseBehaviour : MonoBehaviour
{
    public PlayerAction Inputs => _playerAction;

    private PlayerAction _playerAction;
    private PlayerInput _playerInput;
    private InputControlScheme _scheme;
    private UseContext _useContext;
    private IUse _use = new ScytheItem();

    private int _playerIndex;


    public void OnUse(InputAction.CallbackContext context)
    {
        Vector3 position = transform.position;

        void UseTowardsCursor(InputAction.CallbackContext context)
        {
            _useContext = new UseContext(
                transform,
                _playerIndex,
                context.canceled,
                context.started,
                context.performed
            );

            _use.OnUse(_useContext);
        }

        UseTowardsCursor(context);
    }

    public void ControlsChangedEvent(PlayerInput playerInput)
    {
        _playerIndex = playerInput.playerIndex;
        _scheme = playerInput.user.controlScheme.Value;
    }

    private void Awake()
    {
        _playerAction = new PlayerAction();
        _playerInput = GetComponent<PlayerInput>();
        _use = (IUse)ItemTypeSettings.Instance.ItemTypeChunk["sycthe"].Behaviour;
    }

    private void OnEnable()
    {
        _playerAction.Enable();
        _playerAction.Ground.Use.started += OnUse;
        _playerAction.Ground.Use.performed += OnUse;
        _playerAction.Ground.Use.canceled += OnUse;
        _playerInput.controlsChangedEvent.AddListener(ControlsChangedEvent);
    }

    private void OnDisable()
    {
        _playerAction.Disable();
        _playerAction.Ground.Use.started -= OnUse;
        _playerAction.Ground.Use.performed -= OnUse;
        _playerAction.Ground.Use.canceled -= OnUse;
        _playerInput.controlsChangedEvent.RemoveListener(ControlsChangedEvent);
    }

    private void Update()
    {
        _use.OnUpdate(_useContext);
    }
}
