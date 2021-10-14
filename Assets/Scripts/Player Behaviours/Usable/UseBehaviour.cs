using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Celezt.Time;

public class UseBehaviour : MonoBehaviour
{
    [SerializeField] private PlayerInfo _playerInfo;

    private readonly ItemLabels _itemLabels = new ItemLabels();

    private PlayerInput _playerInput;
    private InputControlScheme _scheme;
    private ItemContext _itemContext;
    private IUse _use;
    private ItemType _itemType;
    private Duration _cooldown;

    private int _playerIndex;

    public void OnUse(InputAction.CallbackContext context)
    {
        if (_itemType == null || _use == null)
            return;

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
        _playerInput = GetComponent<PlayerInput>();

        _playerInfo.Inventory.OnSelectItemCallback += asset =>
        {
            _itemType?.Behaviour?.OnUnequip(_itemContext);  // Unequip current item.

            _itemType = ItemTypeSettings.Instance.ItemTypeChunk[asset.ID];
            _use = (IUse)_itemType.Behaviour;

            _itemContext = new ItemContext(
                _itemType.Labels,
                transform,
                this,
                _itemType.Name,
                _itemType.ID,
                _playerInput.playerIndex
            );
            
            _itemType?.Behaviour?.OnEquip(_itemContext);
        };
    }

    private void Update()
    {
        _itemType?.Behaviour?.OnUpdate(_itemContext);
    }
}
