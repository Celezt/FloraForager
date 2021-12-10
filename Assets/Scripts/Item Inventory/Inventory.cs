using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public class Inventory
{
    public IReadOnlyList<ItemAsset> Items => _items;
    public ItemAsset SelectedItem => _selectedItem;
    public int SelectedIndex => _selectedIndex;

    public event Action OnInventoryDestroyCallback = delegate { };
    public event Action<int, ItemAsset> OnItemChangeCallback = delegate { };
    public event Action<List<ItemAsset>> OnInventoryInitalizeCallback = delegate { };
    public event Action<int, int, ItemAsset, ItemAsset> OnItemMoveCallback = delegate { };
    public event Action<int, ItemAsset> OnAddItemCallback = delegate { };
    public event Action<int, ItemAsset> OnRemoveItemCallback = delegate { };
    public event Action<int, ItemAsset> OnSelectItemCallback = delegate { };

    private ItemAsset _selectedItem;
    private int _selectedIndex = int.MinValue;

    [NonSerialized, ShowInInspector]
    private List<ItemAsset> _items = new List<ItemAsset>(); // Change

    /// <summary>
    /// Select first existing item.
    /// </summary>
    public void SelectFirst() => TrySelectItem(0);

    /// <summary>
    /// Try select at index. If no item exist, try next to it and continue until one is found.
    /// </summary>
    public void TrySelectItem(int index)
    {
        for (int i = index; i < _items.Count; i++)
            if (!string.IsNullOrEmpty(_items[i].ID))
            {
                SetSelectedItem(i);
                break;
            }
    }

    public void SetSelectedItem(int index) 
    {
        if (string.IsNullOrEmpty(_items[index].ID))
            return;

        if (index == _selectedIndex)    // If not already selected.
            return;

        _selectedItem = _items[index];
        _selectedIndex = index;
        OnSelectItemCallback.Invoke(index, _items[index]);
    }

    public ItemAsset Get(int index) => _items[index];

    /// <summary>
    /// Inserts first in existing Indices until full. After that, starts filling in new Indices. Abort the insert if there is not enough space.
    /// </summary>
    /// <returns>If inventory is full.</returns>
    public bool Insert(string id, int amount) => Insert(new ItemAsset { ID = id, Amount = amount });
    /// <summary>
    /// Inserts first in existing Indices until full. After that, starts filling in new Indices. Abort the insert if there is not enough space.
    /// </summary>
    /// <returns>If inventory is full.</returns>
    public bool Insert(ItemAsset item)
    {
        if (FindEmptySpace(item.ID) < item.Amount)     // If not enough space to fill.
            return false;

        return InsertUntilFull(item);
    }
    /// <summary>
    /// Inserts first in existing Indices until full. After that, starts filling in new Indices. Does it until it is done or the
    /// inventory is full.
    /// </summary>
    /// <returns>If inventory is full.</returns>
    public bool InsertUntilFull(string id, int amount) => InsertUntilFull(new ItemAsset { ID = id, Amount = amount });
    /// <summary>
    /// Inserts first in existing Indices until full. After that, starts filling in new Indices. Does it until it is done or the
    /// inventory is full.
    /// </summary>
    /// <returns>If inventory is full.</returns>
    public bool InsertUntilFull(ItemAsset item)
    {
        int amountToAdd = item.Amount;

        if (!ItemTypeSettings.Instance.ItemTypeChunk.TryGetValue(item.ID, out ItemType itemType))    // If it exit.
        {
            Debug.LogError($"{item.ID} does not exist");
            return false;
        }

        int stack = (int)itemType.ItemStack;

        {
            List<(int, int)> foundItems = FindAll(item.ID);

            // Fill existing locations.
            for (int i = 0; i < foundItems.Count; i++)
            {
                int addAmount = Mathf.Clamp(stack - foundItems[i].Item2, 0, stack);

                ItemAsset itemAsset = _items[foundItems[i].Item1];
                itemAsset.Amount += Mathf.Clamp(addAmount, 0, amountToAdd);

                OnAddItemCallback.Invoke(i, itemAsset);
                _items[foundItems[i].Item1] = itemAsset;
                OnItemChangeCallback.Invoke(foundItems[i].Item1, itemAsset);

                if (amountToAdd - addAmount <= 0)
                    return true;
                else
                    amountToAdd -= addAmount;
            }
        }

        // Fill new locations.
        while (FindFirstEmptySlot(out int newIndex))
        {
            ItemAsset itemAsset = _items[newIndex];
            itemAsset.ID = item.ID;
            itemAsset.Amount = Mathf.Clamp(stack, 0, amountToAdd);

            OnAddItemCallback.Invoke(newIndex, itemAsset);
            _items[newIndex] = itemAsset;
            OnItemChangeCallback.Invoke(newIndex, itemAsset);

            if (amountToAdd - stack <= 0)
                return true;
            else
                amountToAdd -= stack;
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
                OnItemChangeCallback.Invoke(index, itemAsset);

                if (index == _selectedIndex && itemAsset.Amount <= 0)
                    TrySelectItem(index + 1);

                return true;
            }
        }
        return false;
    }

    public bool RemoveAt(int index)
    {
        if (index < _items.Count)
        {
            ItemAsset empty = new ItemAsset { };
            _items[index] = empty;
            OnRemoveItemCallback.Invoke(index, empty);
            OnItemChangeCallback.Invoke(index, empty);

            if (index == _selectedIndex)
                SelectFirst();

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
                _items[items[i].Item1] = itemAsset;
                OnRemoveItemCallback.Invoke(items[i].Item1, itemAsset);
                OnItemChangeCallback.Invoke(items[i].Item1, itemAsset);

                break;
            }
        }
    }

    public void Swap(int firstIndex, int secondIndex)
    {
        ItemAsset item = _items[firstIndex];
        _items[firstIndex] = _items[secondIndex];
        _items[secondIndex] = item;
        OnItemChangeCallback.Invoke(firstIndex, _items[secondIndex]);
        OnItemMoveCallback.Invoke(secondIndex, firstIndex, item, _items[firstIndex]);
        OnItemChangeCallback.Invoke(secondIndex, item);
        OnItemMoveCallback.Invoke(firstIndex, secondIndex, _items[firstIndex], item);
    }

    /// <summary>
    /// Find all instances of that type.
    /// </summary>
    /// <returns>(index, amount)</returns>
    public List<(int,int)> FindAll(string id)
    {
        List<(int, int)> found = new List<(int, int)>();
        for (int i = 0; i < _items.Count; i++)
        {
            if (!string.IsNullOrEmpty(_items[i].ID) && _items[i].ID == id)
                found.Add((i, _items[i].Amount));
        }
        return found;
    }

    public bool Find(string id) 
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (!string.IsNullOrEmpty(_items[i].ID))
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
        int amount = 0;
        for (int i = 0; i < _items.Count; i++)
        {
            if (!string.IsNullOrEmpty(_items[i].ID) && _items[i].ID == id)
                amount += _items[i].Amount;
        }
        return amount;
    }

    /// <summary>
    /// Find amount of potential space left to fill of that type.
    /// </summary>
    public int FindEmptySpace(string id)
    {
        int stack = ItemTypeSettings.Instance.ItemStackChunk[id];
        int amountLeft = 0;
        for (int i = 0; i < _items.Count; i++)
        {
            if (string.IsNullOrEmpty(_items[i].ID))     // Empty space.
                amountLeft += stack;
            else if (_items[i].ID == id)                // Existing amount that can be filled.
                amountLeft += Mathf.Clamp(stack - _items[i].Amount, 0, stack);
        }
        return amountLeft;
    }

    /// <summary>
    /// Find enough of that type.
    /// </summary>
    public bool FindEnough(string id, int amount) => amount <= FindAmount(id);

    public bool FindFirstEmptySlot(out int index) 
    {
        for (index = 0; index < _items.Count; index++)
        {
            if (string.IsNullOrEmpty(_items[index].ID))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Find all instances with given labels.
    /// </summary>
    /// <returns>(index, amount)</returns>
    public List<(int, int)> FindAllByLabels(ItemLabels itemLabels)
    {
        List<(int, int)> found = new List<(int, int)>();
        for (int i = 0; i < _items.Count; ++i)
        {
            if (string.IsNullOrEmpty(_items[i].ID))
                continue;

            List<string> labels = ItemTypeSettings.Instance.ItemLabelChunk[_items[i].ID];

            foreach (string label in labels)
            {
                if (Enum.TryParse(label, true, out ItemLabels itemLabel) && itemLabels.HasFlag(itemLabel))
                {
                    found.Add((i, _items[i].Amount));
                    break;
                }
            }
        }
        return found;
    }

    public void Initialize(List<ItemAsset> items)
    {
        for (int i = 0; i < items.Count; ++i)
        {
            _items.Add(items[i]);
        }
        OnInventoryInitalizeCallback.Invoke(_items);
    }

    private void OnDestroy()
    {
        OnInventoryDestroyCallback.Invoke();
    }

    private ItemAsset Swap(int index, ItemAsset newItem)
    {
        ItemAsset item = _items[index];
        _items[index] = newItem;
        OnItemChangeCallback.Invoke(index, newItem);
        return item;
    }

    /// <summary>
    /// Merger two items of the same type.
    /// </summary>
    /// <returns>If same type.</returns>
    public static bool Merge(int fromIndex, int toIndex, Inventory fromInventory, Inventory toInventory)
    {
        ItemAsset fromItem = fromInventory._items[fromIndex];
        ItemAsset toItem = toInventory._items[toIndex];

        if (fromItem.ID != toItem.ID)                               // If not the same type.
            return false;

        if (string.IsNullOrEmpty(fromItem.ID))
            return false;

        int stack = ItemTypeSettings.Instance.ItemStackChunk[fromItem.ID];

        if (fromItem.Amount == stack || toItem.Amount == stack)     // If at least one of them are full.
            return false;

        if (toItem.Amount + fromItem.Amount <= stack)
        {
            toItem.Amount += fromItem.Amount;
            fromItem = new ItemAsset { };
            fromInventory._items[fromIndex] = fromItem;
            toInventory._items[toIndex] = toItem;

            fromInventory.OnItemMoveCallback.Invoke(fromIndex, toIndex, fromItem, toItem);
        }
        else
        {
            int emptyAmount = stack - toItem.Amount;
            toItem.Amount = stack;
            fromItem.Amount -= emptyAmount;

            fromInventory._items[fromIndex] = fromItem;
            toInventory._items[toIndex] = toItem;
        }

        fromInventory.OnItemChangeCallback(fromIndex, fromItem);
        toInventory.OnItemChangeCallback(toIndex, toItem);

        return true;
    }

    /// <summary>
    /// Swap two items index.
    /// </summary>
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
