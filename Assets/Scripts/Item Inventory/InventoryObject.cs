using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public event Action<int> OnItemChangeCallback = delegate { };
    public event Action<int,ItemAsset> OnAddItemCallback = delegate { };
    public event Action<int, ItemAsset> OnRemoveItemCallback = delegate { };
    public event Action<ItemAsset> OnSelectItemCallback = delegate { };
    public ItemAsset SelectedItem;
    [NonSerialized, ShowInInspector]
    public List<ItemAsset> Container = new List<ItemAsset>(); // Change

    public void SetSelectedItem(ItemAsset item) 
    {
        SelectedItem = item;
        OnSelectItemCallback.Invoke(item);
    }
    public bool IsFull { get; set; }
    public bool AddItem(ItemAsset item)
    {
        if (!IsFull)
        {
            int pos = ExistsAt(item.ID);
            if (pos != -1) // It exists
            {
                ItemAsset tmp = Container[pos];
                // Check if new amount > max amount
                tmp.Amount += item.Amount;
                Container[pos] = tmp;
                OnAddItemCallback.Invoke(pos,tmp);
                OnItemChangeCallback.Invoke(pos);
            }
            else
            {
                int tmp = FindFirstEmptySlot();
                Container[tmp] = item;
                OnAddItemCallback.Invoke(tmp,item);
                OnItemChangeCallback.Invoke(pos);
            }
            return true;
        }
        else
        {
            /*
            if (stackable)
            {
                Find all slots of the same id that are not empty
                if(Found stackables)
            }
            {
            
            }
            else
            {
            return false;
            }
             else
            {
            return false;
            }
             */
        }
        return false;
    }

    public bool RemoveAt(int pos) 
    {
        if (pos < Container.Count)
        {
            OnRemoveItemCallback.Invoke(pos, Container[pos]);
            Container[pos] = new ItemAsset();
            OnItemChangeCallback.Invoke(pos);
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
                ItemAsset itemAsset = Container[items[i].Item1];
                itemAsset.Amount -= amountToRemove;
                Container[items[i].Item1] = itemAsset;
                OnItemChangeCallback.Invoke(items[i].Item1);
            }
        }
    }
    public void Swap(int pos, int pos2)
    {
        ItemAsset holder = Container[pos];
        Container[pos] = Container[pos2];
        Container[pos2] = holder;
        OnItemChangeCallback.Invoke(pos);
        OnItemChangeCallback.Invoke(pos2);
    }
    public int ExistsAt(string ID) 
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (!Container[i].ID.IsNullOrEmpty())
            {
                if (Container[i].ID == ID)
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
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].ID != null && Container[i].ID == ID)
            {
                tmp.Add((i, Container[i].Amount));
            }
        }
        return tmp;

    }
    public bool Find(string ID) 
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (!Container[i].ID.IsNullOrEmpty())
            {
                if (Container[i].ID == ID)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int FindAmount(string ID) 
    {
        int tmp = 0;
        for (int i = 0; i < Container.Count; i++)
        {
            if (Container[i].ID != null && Container[i].ID == ID)
            {
                tmp+=Container[i].Amount;
            }
        }
        return tmp;
    }
    public bool FindEnough(string ID, int amount) 
    {        
        return amount <= FindAmount(ID);
    }
    public int FindFirstEmptySlot() 
    {
        for (int i = 0; i < Container.Count; i++)
        {
            if (!Container[i].ID.IsNullOrEmpty())
            {
                return i;
            }
        }
        return -1;
    }

}
