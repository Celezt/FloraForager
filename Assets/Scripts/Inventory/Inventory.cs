using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int maxSlots;
    private int nrItems;
    public int gold;
    public Item[] items;
    private bool isFull;

    public void IniFromFile()
    {

    }
    public void Initiate()
    {
        items = new Item[maxSlots];
    }
    public bool AddItem(Item item)
    {
        if (!isFull) // Not full, can add
        {
            if (Exists(item.id)) // Aready occupies a slot
            {
                if (Stacking(item)) // If stacking possible
                {
                    return true; // stacking successful
                }
            }
            return AddToNewSlot(item); // Adds to empty slot successfully
        }
        else if (Exists(item.id)) // Inventory full. Item might be stackable - check if it exists
        {
            return Stacking(item); // If items have been stacked or not
        }

        Debug.Log("Inventory full");
        return false; // Item not successfully added
    }
    public bool RemoveAmount(int id, int amount)
    {
        if (Exists(id))// It exists
        {

        }
        return false; // Does not exist or cannot remove that amount
    }

    private bool Stacking(Item item)  // Tries to stack the item
    {
        if (item.IsFull) // Check if you can stack the item
        {
            LinkedList<int> stackables = FindStackables(item.id);
            int amountLeft = item.Amount;
            int maxAmount = item.MaxAmount;
            if (stackables.Count != 0) // There are stackables slots
            {
                for (int i = 0; i < stackables.Count; i++)
                {
                    int itemLeft = amountLeft - maxAmount - items[stackables.First.Value].Amount;
                    if (0 >= itemLeft) // Fits
                    {
                        items[stackables.First.Value].Amount += itemLeft;                        
                        return true;
                    }
                    else if (0 < itemLeft)
                    {

                    }
                    else // itemLeft = 0
                    {
                        return true; // Stacked successfully
                    }
                    stackables.RemoveFirst();
                }
            }
        }
        return false; // Can't stack
    }

    // Done
    private void UpdateFullness() // Check if it's full
    {
        if (nrItems == maxSlots) // If inventory is full 
        {
            isFull = true; // Is full
        }
        else
        {
            isFull = false; // Is not full
        }
    }
    private bool AddToNewSlot(Item item)
    {
        if (!isFull) // Not full
        {
            items[FindFirstEmptySlot()] = new Item(item); // Does not Exist in a slot and is not stackable, Add to an empty slot
            ++nrItems; // Added one item
            UpdateFullness(); // If isFull == true
            return true; // Item successfully added to one slot            
        }
        return false; // Is full
    }
    private int FindFirstEmptySlot()
    {
        if (!isFull) // Not full
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (items[i] == null) // Is empty
                {
                    return i; // Return empty slot number
                }
            }
        }

        return -1; // Is full
    }
    private bool Exists(int id) // If item exists in inventory
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (items[i].id == id)
            {
                return true;
            }
        }
        
        return false;
    }
    private LinkedList<int> FindStackables(int id) // Finding Slots with the same ID that are not full
    {
        LinkedList<int> stackables = new LinkedList<int>();
        for (int i = 0; i < maxSlots; i++) // Searches through the whole array
        {
            if (items[i] != null) // If the slot is occupied
            {
                if (items[i].id == id && items[i].IsFull != false) // Found identical item 
                {
                    stackables.AddLast(i);
                }
            }
        }
        return stackables; // returns a list filled with positions of the same stackable items 
    }


}
