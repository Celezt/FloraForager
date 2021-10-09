using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Celezt.Time;

public class UseBehaviour : MonoBehaviour
{
    public PlayerAction Inputs => _playerAction;

    private readonly ItemLabels _itemLabels = new ItemLabels();

    private PlayerAction _playerAction;
    private PlayerInput _playerInput;
    private InputControlScheme _scheme;
    private ItemContext _itemContext;
    private IUse _use;
    private ItemType _itemType;
    private Duration _cooldown;

    private int _playerIndex;

    public void OnUse(InputAction.CallbackContext context)
    {
        if (!_cooldown.IsActive)
        {
            if (context.started)
                _cooldown = new Duration(_use.Cooldown);

            void UseTowardsCursor()
            {
                UseContext useContext = new UseContext(
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

                foreach (IUsable usable in _use.OnUse(useContext))
                    foreach (string label in useContext.labels)
                        if (usable.Filter(_itemLabels).Contains(label))
                        {
                            usable.OnUse(new UsedContext(
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
                                ));
                            break;
                        }
            }

            UseTowardsCursor();
        }
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
        _itemType = ItemTypeSettings.Instance.ItemTypeChunk["porock"];
        _use = (IUse)_itemType.Behaviour;

        _itemContext = new ItemContext(
            _itemType.Labels,
            transform,
            this,
            _itemType.Name,
            _itemType.ID,
            _playerIndex
        );

        _itemType.Behaviour.Initialize(_itemContext);
    }

    private void Start()
    {
        _itemType.Behaviour.OnEquip(_itemContext);
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

        _itemType.Behaviour.OnUnequip(_itemContext);
    }

    private void Update()
    {
        _itemContext = new ItemContext(
            _itemType.Labels,
            transform,
            this,
            _itemType.Name,
            _itemType.ID,
            _playerIndex
            );

        _itemType.Behaviour.OnUpdate(_itemContext);
    }
}
