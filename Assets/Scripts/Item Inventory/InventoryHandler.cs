using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryHandler : MonoBehaviour
{
    public Inventory Inventory => _inventory;
    public string ID => _id;

    [SerializeField]
    private string _id;

    private Inventory _inventory;

    private ItemSlot[] _slots;

    void Awake()
    {
        GameManager.Instance.Stream.Get<Inventory>(_id).TryGetTarget(out _inventory);

        _inventory.OnItemChangeCallback += (int i) =>
        {
            ItemTypeSettings settings = ItemTypeSettings.Instance;

            if (!(i >= _slots.Length))
            {
                _slots[i].Item = _inventory.Items[i];
                if (_slots[i].Item.ID != null)
                {
                    if (settings.ItemIconChunk.TryGetValue(_slots[i].Item.ID, out Sprite sprite))
                    {
                        _slots[i].Icon.sprite = sprite;
                    }
                    else
                    {
                        _slots[i].Icon.sprite = null;
                    }
                }
                else
                {
                    _slots[i].Icon.sprite = null;
                }
                
                _slots[i].Amount.text = _inventory.Items[i].Amount.ToString();
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
                        _slots[i].Icon.sprite = sprite;
                    if (settings.ItemNameChunk.TryGetValue(items[i].ID, out string name))
                        _slots[i].Name = name;

                    _slots[i].Item = items[i];
                    _slots[i].Amount.text = items[i].Amount.ToString();
                }
            }
        };
    }
}
