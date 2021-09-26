using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : ScriptableObject
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

    public bool IsFull { get => isFull; }

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
    }
}
