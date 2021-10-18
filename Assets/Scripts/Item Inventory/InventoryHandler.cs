using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour
{
    public Inventory Inventory
    {
        get => _inventory;
        set => _inventory = value;
    }
    public bool IsItemSelectable
    {
        get => _isItemSelectable;
        set => _isItemSelectable = value;
    }
    public int SlotAmount => _slots.Length;

    public event Action OnInventoryInitalizedCallback = delegate { };

    private ItemSlot[] _slots;

    private Inventory _inventory;

    private bool _isItemSelectable;

    private void Start()
    {
        void SetEmptyIcon(int index)
        {
            Image icon = _slots[index].Icon;
            icon.sprite = null;
            Color color = icon.color;
            color.a = 0;
            icon.color = color;
        }
        void SetIcon(int index, Sprite sprite)
        {
            Image icon = _slots[index].Icon;
            icon.sprite = sprite;
            Color color = icon.color;
            color.a = 1;
            icon.color = color;
        }

        _inventory.OnItemChangeCallback += (int i) =>
        {
            ItemTypeSettings settings = ItemTypeSettings.Instance;

            if (!(i >= _slots.Length))
            {
                _slots[i].Item = _inventory.Items[i];
                if (!string.IsNullOrEmpty(_slots[i].Item.ID))
                {
                    if (settings.ItemIconChunk.TryGetValue(_slots[i].Item.ID, out Sprite sprite))
                        SetIcon(i, sprite);
                    else
                        SetEmptyIcon(i);
                }
                else
                    SetEmptyIcon(i);

                _slots[i].Amount.text = _inventory.Items[i].Amount > 0 ? _inventory.Items[i].Amount.ToString() : "";
            }
        };

        _inventory.OnInventoryDeserializeCallback += (items) =>
        {
            ItemTypeSettings settings = ItemTypeSettings.Instance;

            _slots = GetComponentsInChildren<ItemSlot>();

            int currentCount = items.Count;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (currentCount <= i)  // Resize to fit the amount of slots.
                    items.Add(new ItemAsset());

                _slots[i].Index = i;
                _slots[i].InventoryHandler = this;

                if (!string.IsNullOrEmpty(items[i].ID))
                {
                    if (settings.ItemIconChunk.TryGetValue(items[i].ID, out Sprite sprite))
                        SetIcon(i, sprite);
                    else
                        SetEmptyIcon(i);

                    if (settings.ItemNameChunk.TryGetValue(items[i].ID, out string name))
                        _slots[i].Name = name;

                    _slots[i].Item = items[i];
                    _slots[i].Amount.text = _inventory.Items[i].Amount > 0 ? _inventory.Items[i].Amount.ToString() : "";
                }
            }

            OnInventoryInitalizedCallback.Invoke();
        };
    }
}
