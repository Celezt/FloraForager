using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Celezt.Time;
using UnityEngine.EventSystems;

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

    private int _slotIndex;
    private int _amount;

    public void ConsumeCurrentItem(int amount)
    {
        _playerInfo.Inventory.RemoveAt(_slotIndex, amount);
    }

    public void Unequip()
    {
        _itemType?.Behaviour?.OnUnequip(_itemContext);  // Unequip current item.
        _use = null;
        _itemType = null;
        OnDrawGizmosAction = null;
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (CanvasUtility.IsPointerOverUIElement()) // Skip if pointing over a UI element.
            return;

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
                    _playerInfo,
                    _itemType.Name,
                    _itemType.ID,
                    _playerInput.playerIndex,
                    _slotIndex,
                    _amount,
                    context.canceled,
                    context.started,
                    context.performed
                );

                foreach (IUsable usable in _use.OnUse(useContext))
                {
                    foreach (string label in useContext.labels)
                    {
                        if (usable.Filter(_itemLabels).Contains(label))
                        {
                            UsedContext usedContext = new UsedContext(
                                _use,
                                _itemType.Labels,
                                transform,
                                this,
                                _itemType.Name,
                                _itemType.ID,
                                _playerInput.playerIndex,
                                context.canceled,
                                context.started,
                                context.performed
                            );

                            //if (usable is IDestructableObject && _use is IDestructor)                     // Do damage.
                            //{
                            //    IDestructableObject destructable = usable as IDestructableObject;
                            //    destructable.OnDamage(_use as IDestructor, destructable, usedContext);

                            //    if (destructable.Durability <= 0)                                   // If destroyed.
                            //        destructable.OnDestruction(usedContext);
                            //}

                            usable.OnUse(usedContext);
                            break;
                        }
                    }
                }
            }

            UseTowardsCursor();
        }
    }

    public void ControlsChangedEvent(PlayerInput playerInput)
    {
        _scheme = playerInput.user.controlScheme.Value;
    }

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();

        _playerInfo.Inventory.OnInventoryInitalizeCallback += (_) =>
        {
            _slotIndex = _playerInfo.Inventory.SelectedIndex;

            ItemTypeContext itemTypeContext = new ItemTypeContext(
                transform,
                this,
                _playerInfo,
                _playerInput.playerIndex,
                _slotIndex
            );

            IReadOnlyDictionary<string, ItemType> itemTypeChunk = ItemTypeSettings.Instance.ItemTypeChunk;  // Activate all OnInitalize.
            foreach (KeyValuePair<string, ItemType> itemType in itemTypeChunk)
                itemType.Value?.Behaviour?.OnInitialize(itemTypeContext);
        };
        
        _playerInfo.Inventory.OnItemMoveCallback += (beforeIndex, afterIndex, beforeItem, afterItem) =>
        {
            if (beforeIndex == _slotIndex)
                _slotIndex = afterIndex;
        };

        _playerInfo.Inventory.OnSelectItemCallback += (index, item) =>
        {
            Unequip();

            if (string.IsNullOrEmpty(item.ID) || !ItemTypeSettings.Instance.ItemTypeChunk.ContainsKey(item.ID))
                return;

            _itemType = ItemTypeSettings.Instance.ItemTypeChunk[item.ID];

            if (_itemType.Behaviour != null && _itemType.Behaviour is IUse)                // If item has implemented IUse.
                _use = (IUse)_itemType.Behaviour;

            _slotIndex = index;
            _amount = item.Amount;

            _itemContext = new ItemContext(
                _itemType.Labels,
                transform,
                this,
                _playerInfo,
                _itemType.Name,
                _itemType.ID,
                _playerInput.playerIndex,
                _slotIndex,
                _amount
            );

            _itemType?.Behaviour?.OnEquip(_itemContext);
        };

        _playerInfo.Inventory.OnRemoveItemCallback += (index, item) =>
        {
            if (item.Amount == 0 && _cooldowns.ContainsKey(index))
                _cooldowns.Remove(index);

            if (index == _slotIndex)
            {
                if (item.Amount > 0)    //  Update amount if not empty.
                    _amount = item.Amount;
                else
                    Unequip();
            }
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
            _playerInfo,
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
