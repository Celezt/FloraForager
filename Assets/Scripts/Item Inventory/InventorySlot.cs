using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InventorySlot
{
    
    public ItemAsset item;
    public bool isFull;

   

    public InventorySlot(ItemAsset i)
    {
        item = i;
    }
    public InventorySlot(InventorySlot other)
    {
        item = other.item;
        isFull = other.IsFull;
    }

    public void RemoveFromSlot() // Removes from slot
    {
        
    }

    public bool IsFull { get => isFull; }

    public bool AddAmount(int value) // Returns if adding was successful
    {
        bool added = false;
        if (item.Amount + value < ItemDatabase.Database[item.ID].MaxAmount) // lower than max value
        {
            item.Amount += value;
            added = true;
        }
        else if (item.Amount + value == ItemDatabase.Database[item.ID].MaxAmount) // Equals max value
        {
            item.Amount += value;
            isFull = true;
            added = true;
        }
        
        return added; // Over max value - No Add        
    }
}
