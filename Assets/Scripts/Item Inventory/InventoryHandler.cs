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

    public IReadOnlyList<ItemSlot> Slots => _slots;

    public event Action OnInventoryInitalizedCallback = delegate { };

    private ItemSlot[] _slots;

    private Inventory _inventory;
    private ItemTypeSettings _settings;

    private bool _isItemSelectable;

    private void Start()
    {
        _settings = ItemTypeSettings.Instance;

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
        void SetTextAmount(int index)
        {
            _slots[index].Amount.text = _inventory.Items[index].Amount > 1 ? _inventory.Items[index].Amount.ToString() : "";
        }

        _inventory.OnItemChangeCallback += (index, item) =>
        {
            if (!(index >= _slots.Length))
            {
                _slots[index].Item = _inventory.Items[index];
                if (!string.IsNullOrEmpty(_slots[index].Item.ID))
                {
                    if (_settings.ItemIconChunk.TryGetValue(_slots[index].Item.ID, out Sprite sprite))
                        SetIcon(index, sprite);
                    else
                        SetEmptyIcon(index);
                }
                else
                    SetEmptyIcon(index);

                SetTextAmount(index);
            }
        };

        _inventory.OnInventoryInitalizeCallback += (items) =>
        {
            _slots = GetComponentsInChildren<ItemSlot>(true);

            int currentCount = items.Count;
            for (int i = 0; i < _slots.Length; i++)
            {
                if (currentCount <= i)  // Resize to fit the amount of slots.
                    items.Add(new ItemAsset());

                _slots[i].Index = i;
                _slots[i].InventoryHandler = this;

                if (!string.IsNullOrEmpty(items[i].ID))
                {
                    if (_settings.ItemIconChunk.TryGetValue(items[i].ID, out Sprite sprite))
                        SetIcon(i, sprite);
                    else
                        SetEmptyIcon(i);

                    if (_settings.ItemNameChunk.TryGetValue(items[i].ID, out string name))
                        _slots[i].Name = name;

                    _slots[i].Item = items[i];
                    SetTextAmount(i);
                }
            }

            OnInventoryInitalizedCallback.Invoke();
        };
    }
}
