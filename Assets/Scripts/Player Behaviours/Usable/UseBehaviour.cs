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
    private IUse _use;
    private ItemType _itemType;
    private ItemLabels _itemLabels = new ItemLabels();

    private int _playerIndex;

    public void OnUse(InputAction.CallbackContext context)
    {
        Vector3 position = transform.position;

        void UseTowardsCursor(InputAction.CallbackContext context)
        {
            _useContext = new UseContext(
                _use,
                _itemType.Labels,
                transform,
                this,
                _itemType.Name,
                _itemType.ID,
                _playerIndex,
                context.canceled,
                context.started,
                context.performed
            );
            
            foreach (IUsable usable in _use.OnUse(_useContext))
                foreach (string label in _useContext.labels)
                    if (usable.Filter(_itemLabels).Contains(label))
                    {
                        usable.OnUse(_useContext);
                        break;
                    }
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
        _itemType = ItemTypeSettings.Instance.ItemTypeChunk["sycthe"];
        _use = (IUse)_itemType.Behaviour;

        _useContext = new UseContext(
            _use,
            _itemType.Labels,
            transform,
            this,
            _itemType.Name,
            _itemType.ID,
            _playerIndex,
            false,
            false,
            false
        );

        _itemType.Behaviour.Initialize(_useContext);
    }

    private void Start()
    {
        _use.OnEquip(_useContext);
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

        _use.OnUnequip(_useContext);
    }

    private void Update()
    {
        _use.OnUpdate(_useContext);
    }
}
