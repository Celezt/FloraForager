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

    public Action OnDrawGizmosAction = delegate { };

    private PlayerInput _playerInput;
    private InputControlScheme _scheme;
    private ItemContext _itemContext;
    private IUse _use;
    private ItemType _itemType;

    private Dictionary<int, Duration> _cooldowns = new Dictionary<int, Duration>();

    private int _playerIndex;
    private int _slotIndex;
    private int _amount;

    public void ConsumeCurrentItem(int amount)
    {
        _playerInfo.Inventory.RemoveAt(_slotIndex, amount);
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (_itemType == null || _use == null)
            return;

        if (!_cooldowns.ContainsKey(_slotIndex))
            _cooldowns.Add(_slotIndex, Duration.Empty);

        if (!_cooldowns[_slotIndex].IsActive)
        {
            if (context.started)
                _cooldowns[_slotIndex] = new Duration(_use.Cooldown);

            void UseTowardsCursor()
            {
                UseContext useContext = new UseContext(
                    _itemType.Labels,
                    transform,
                    this,
                    _itemType.Name,
                    _itemType.ID,
                    _playerIndex,
                    _slotIndex,
                    _amount,
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

        _playerInfo.Inventory.OnSelectItemCallback += (index, asset) =>
        {
            _itemType?.Behaviour?.OnUnequip(_itemContext);  // Unequip current item.
            _use = null;
            _itemType = null;
            OnDrawGizmosAction = null;

            if (string.IsNullOrEmpty(asset.ID) || !ItemTypeSettings.Instance.ItemTypeChunk.ContainsKey(asset.ID))
                return;

            _itemType = ItemTypeSettings.Instance.ItemTypeChunk[asset.ID];

            if (_itemType.Behaviour != null && _itemType.Behaviour is IUse)                // If item has implemented IUse.
                _use = (IUse)_itemType.Behaviour;

            _slotIndex = index;
            _amount = asset.Amount;

            _itemContext = new ItemContext(
                _itemType.Labels,
                transform,
                this,
                _itemType.Name,
                _itemType.ID,
                _playerInput.playerIndex,
                _slotIndex,
                _amount
            );
            
            _itemType?.Behaviour?.OnEquip(_itemContext);
        };

        _playerInfo.Inventory.OnRemoveItemCallback += (index, asset) =>
        {
            if (asset.Amount == 0 && _cooldowns.ContainsKey(index))
                _cooldowns.Remove(index);

            if (index == _slotIndex)
                _amount = asset.Amount;
        };
    }

    private void Update()
    {
        if (_itemType == null)
            return;

        _itemContext = new ItemContext(
            _itemType.Labels,
            transform,
            this,
            _itemType.Name,
            _itemType.ID,
            _playerInput.playerIndex,
            _slotIndex,
            _amount
        );

        _itemType?.Behaviour?.OnUpdate(_itemContext);
    }

    private void OnDrawGizmos()
    {
        if (DebugManager.DebugMode && OnDrawGizmosAction != null)     // Only invoke on debug mode.
            OnDrawGizmosAction.Invoke();
    }
}
