using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class Inventory : ScriptableObject
{
    public IReadOnlyList<ItemAsset> Items => _items;
    public ItemAsset SelectedItem => _selectedItem;
    public int SelectedIndex => _selectedIndex;

    public event Action OnInventoryDestroyCallback = delegate { };
    public event Action<int> OnItemChangeCallback = delegate { };
    public event Action<List<ItemAsset>> OnInventoryDeserializeCallback = delegate { };
    public event Action<int, int, ItemAsset, ItemAsset> OnItemMoveCallback = delegate { };
    public event Action<int, ItemAsset> OnAddItemCallback = delegate { };
    public event Action<int, ItemAsset> OnRemoveItemCallback = delegate { };
    public event Action<int, ItemAsset> OnSelectItemCallback = delegate { };

    private ItemAsset _selectedItem;
    private int _selectedIndex = int.MinValue;

    [NonSerialized, ShowInInspector]
    public List<ItemAsset> _items = new List<ItemAsset>(); // Change
    public bool IsFull { get; set; }

    public void SetSelectedItem(int index) 
    {
        if (string.IsNullOrEmpty(_items[index].ID))
            return;

        _selectedItem = _items[index];
        _selectedIndex = index;
        OnSelectItemCallback.Invoke(index, _items[index]);
    }

    public ItemAsset Get(int index) => _items[index];
    public bool Insert(ItemAsset item)
    {
        if (!IsFull)
        {
            int pos = ExistsAt(item.ID);
            if (pos != -1) // It exists
            {
                ItemAsset tmp = _items[pos];
                // Check if new amount > max amount
                tmp.Amount += item.Amount;
                _items[pos] = tmp;
                OnAddItemCallback.Invoke(pos,tmp);
                OnItemChangeCallback.Invoke(pos);
            }
            else
            {
                int tmp = FindFirstEmptySlot();
                _items[tmp] = item;
                OnAddItemCallback.Invoke(tmp,item);
                OnItemChangeCallback.Invoke(tmp);
            }
            return true;
        }
        else
        {

        }
        return false;
    }

    public bool RemoveAt(int index, int amount) 
    {
        if (index < _items.Count)
        {
            ItemAsset itemAsset = _items[index];
            if (amount <= itemAsset.Amount)
            {
                itemAsset.Amount -= amount;
                OnRemoveItemCallback.Invoke(index, itemAsset);
                _items[index] = itemAsset.Amount > 0 ? itemAsset : new ItemAsset { };
                OnItemChangeCallback.Invoke(index);
                return true;
            }
        }
        return false;
    }

    public bool RemoveAt(int index)
    {
        if (index < _items.Count)
        {
            OnRemoveItemCallback.Invoke(index, new ItemAsset { ID = _items[index].ID, Amount = 0});
            _items[index] = new ItemAsset { };
            OnItemChangeCallback.Invoke(index);
            return true;
        }
        return false;
    }

    public void Remove(string id, int amount)
    {
        List<(int, int)> items = FindAll(id);
        int amountToRemove = amount;

        for (int i = items.Count - 1; i >= 0; --i)
        {
            if (items[i].Item2 - amountToRemove <= 0)
            {
                amountToRemove -= items[i].Item2;
                RemoveAt(items[i].Item1);
            }
            else
            {
                ItemAsset itemAsset = _items[items[i].Item1];
                itemAsset.Amount -= amountToRemove;
                OnRemoveItemCallback.Invoke(items[i].Item1, itemAsset);
                _items[items[i].Item1] = itemAsset;
                OnItemChangeCallback.Invoke(items[i].Item1);
            }
        }
    }

    public void Swap(int firstIndex, int secondIndex)
    {
        ItemAsset item = _items[firstIndex];
        _items[firstIndex] = _items[secondIndex];
        _items[secondIndex] = item;
        OnItemChangeCallback.Invoke(firstIndex);
        OnItemMoveCallback.Invoke(secondIndex, firstIndex, item, _items[firstIndex]);
        OnItemChangeCallback.Invoke(secondIndex);
        OnItemMoveCallback.Invoke(firstIndex, secondIndex, _items[firstIndex], item);
    }

    public int ExistsAt(string id) 
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (!_items[i].ID.IsNullOrEmpty())
            {
                if (_items[i].ID == id)
                {
                    return i;
                }
            }
        }
        return -1;
    }
    public List<(int,int)> FindAll(string ID) // pos, amount
    {
        List<(int, int)> tmp = new List<(int, int)>();
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].ID != null && _items[i].ID == ID)
            {
                tmp.Add((i, _items[i].Amount));
            }
        }
        return tmp;

    }
    public bool Find(string id) 
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (!_items[i].ID.IsNullOrEmpty())
            {
                if (_items[i].ID == id)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int FindAmount(string id) 
    {
        int tmp = 0;
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].ID != null && _items[i].ID == id)
            {
                tmp += _items[i].Amount;
            }
        }
        return tmp;
    }
    public bool FindEnough(string id, int amount) 
    {        
        return amount <= FindAmount(id);
    }
    public int FindFirstEmptySlot() 
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (!_items[i].ID.IsNullOrEmpty())
            {
                return i;
            }
        }
        return -1;
    }

    private void Deserialize()
    {
        Addressables.LoadAssetAsync<TextAsset>("inventory").Completed += (handle) =>
        {
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);

            for (int i = 0; i < tmp.Items.Length; i++)
            {
                _items.Add(tmp.Items[i]);
            }

            Addressables.Release(handle);

            OnInventoryDeserializeCallback.Invoke(_items);
        };
    }

    private void Awake()
    {
        Deserialize();
    }

    private void OnDestroy()
    {
        OnInventoryDestroyCallback.Invoke();
    }

    private ItemAsset Swap(int index, ItemAsset newItem)
    {
        ItemAsset item = _items[index];
        _items[index] = newItem;
        OnItemChangeCallback.Invoke(index);
        return item;
    }

    public static void Swap(int firstIndex, int secondIndex, Inventory firstInventory, Inventory secondInventory)
    {
        // If moving selected item.
        if (firstIndex == firstInventory._selectedIndex)
            firstInventory._selectedIndex = secondIndex;
        else if (secondIndex == secondInventory._selectedIndex)
            secondInventory._selectedIndex = firstIndex;

        ItemAsset holder = firstInventory.Swap(firstIndex, secondInventory.Get(secondIndex));
        secondInventory.Swap(secondIndex, holder);

        firstInventory.OnItemMoveCallback.Invoke(firstIndex, secondIndex, holder, secondInventory.Get(secondIndex));
        secondInventory.OnItemMoveCallback.Invoke(secondIndex, firstIndex, secondInventory.Get(secondIndex), holder);
    }
}
