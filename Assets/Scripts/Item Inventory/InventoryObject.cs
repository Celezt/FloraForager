using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public event Action<int> InventoryAction = delegate { };
    //public List<InventorySlot> container = new List<InventorySlot>();
    public ItemAsset[] Container = new ItemAsset[8];
    public bool IsFull { get; set; }
    public bool AddItem(ItemAsset item)
    {
        if (!IsFull)
        {
            bool hasItem = false;
            for (int i = 0; i < Container.Length; i++)
            {
                if (!Container[i].ID.IsNullOrEmpty() && Container[i].ID == item.ID)
                {
                    Container[i].Amount +=item.Amount;
                    hasItem = true;
                    InventoryAction.Invoke(i);
                    break;
                }
            }
            if (!hasItem)
            {
                int tmp = FindFirstEmptySlot();
                Container[tmp] = item;
                InventoryAction.Invoke(tmp);
            }
            return true;
        }
        else
        {

        }
        return false;
    }
    
    public int ExistsAt(string id) 
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (Container[i].ID == id)
            {
                return i;
            }
        }
        return -1;
    }
    public int FindFirstEmptySlot() 
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (!Container[i].ID.IsNullOrEmpty())
            {
                return i;
            }
        }
        return -1;
    }
}

/*
[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    private bool isFull;

    public bool IsEmpty() 
    {
        if (item != null)
        {
            return true;
        }
        return false;
    }
    public InventorySlot(ItemObject _item, int _amount)
    {
        item = _item;
        AddAmount(_amount);
    }
    public InventorySlot(InventorySlot other)
    {
        item = other.item;
        amount = other.amount;
        isFull = other.IsFull;
    }    
    
    public void RemoveFromSlot() // Removes from slot
    {
        item = null;
        amount = 0;
        isFull = false;
    }

    public bool IsFull { get => isFull;}

    public bool AddAmount(int value) // Returns if adding was successful
    {
        bool added = false;
        if (amount + value < item.maxAmount) // lower than max value
        {
            amount += value;
            added = true;
        }
        else if (amount + value == item.maxAmount) // Equals max value
        {
            amount += value;
            isFull = true;
            added = true;
        }
        return added; // Over max value - No Add        
    
}}*/
