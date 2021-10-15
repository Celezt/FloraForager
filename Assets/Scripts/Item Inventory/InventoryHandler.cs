using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryHandler : MonoBehaviour
{
    public Inventory Inventory { get => _inventory; }

    [SerializeField]
    private Inventory _inventory;
    [SerializeField]
    private string _inventoryID;

    private ItemTypeSettings _settings;

    private ItemSlot[] _slots;

    public void SelectItem(ItemSlot itemSlot) 
    {
        _inventory.SetSelectedItem(itemSlot.Index, itemSlot.Item);
    }

    void Start()
    {
        _inventory.OnItemChangeCallback += (int i) =>
        {
            if (!(i >= _slots.Length))
            {
                _slots[i].Item = _inventory.Container[i];
                if (_slots[i].Item.ID != null)
                {
                    if (_settings.ItemIconChunk.TryGetValue(_slots[i].Item.ID, out Sprite sprite))
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
                
                _slots[i].TextMesh.text = _inventory.Container[i].Amount.ToString();
            }
        };

        // This occurs 2 time for hud and player inv!!!
        Addressables.LoadAssetAsync<TextAsset>(_inventoryID).Completed +=(handle)=>
        {
            
            _settings = ItemTypeSettings.Instance;
            
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            _slots = GetComponentsInChildren<ItemSlot>();
            
            // Assigns Items to slots
            for (int i = 0; i < _slots.Length; i++) // Assigns Items to slots
            {                
                _inventory.Container.Add(tmp.Items.Length > i ? tmp.Items[i]: new ItemAsset());
                _slots[i].Index = i;
                _slots[i].GetComponent<ItemSlot>().InventoryManager = this;

                if (_inventory.Container[i].ID != null)
                {
                    _slots[i].Item = _inventory.Container[i];
                    if (_settings.ItemIconChunk.TryGetValue(_slots[i].Item.ID, out Sprite sprite))
                    {
                        _slots[i].Icon.sprite = sprite;
                        
                    }
                    _slots[i].TextMesh.text = _inventory.Container[i].Amount.ToString(); // Can use slot instead?
                }
            }

            Addressables.Release(handle);
        };
    }
}
