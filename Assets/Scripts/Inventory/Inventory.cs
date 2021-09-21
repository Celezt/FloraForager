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

    //Done
    public void IniFromFile()
    {

    }
    public bool RemoveAmount(int id, int amount)
    {
        LinkedList<int> positions = FindAll(id);
        int[] tmp = new int[positions.Count];
        int nrOfSpace = 0;
        int amountToRemove = amount;
        for (int i = 0; i < tmp.Length; i++)
        {
            nrOfSpace += items[tmp[i]].Amount;
        }
        if (nrOfSpace > 0 && nrOfSpace >= amount)
        {
            for (int i = 0; i < tmp.Length; i++)
            {
                if (amountToRemove > 0)
                {
                    if (amountToRemove >= items[tmp[i]].Amount)
                    {
                        amountToRemove -= items[tmp[i]].Amount;
                        RemoveFromSlot(tmp[i]);
                    }
                    else
                    {
                        items[tmp[i]].Amount -= amountToRemove;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        return false; // Does not exist or cannot remove that amount
    }
    public Inventory() 
    {
        items = new Item[maxSlots];
        nrItems = 0;
        maxSlots = 32;
        gold = 0;
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = null;
        }
    }
    public void Initiate()
    {
        items = new Item[maxSlots];
        nrItems = 0;
        maxSlots = 32;
        gold = 0;
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = null;
        }
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
    private bool Stacking(Item item)  // Tries to stack the item
    {
        if (item.IsFull) //If item is stackable
        {
            LinkedList<int> stackables = FindStackables(item); // Find how many items with the same id that have space
            int[] tmp = new int[stackables.Count];
            int nrOfSpace = 0;
            if (tmp.Length > 0) // Found stackable slots
            {
                for (int i = 0; i < tmp.Length; i++) // Copy linkedlist to array
                {
                    tmp[i] = stackables.First.Value;
                    nrOfSpace += items[stackables.First.Value].MaxAmount - items[stackables.First.Value].Amount; // Adds amount of empty space
                    stackables.RemoveFirst();
                }

                if (nrOfSpace >= item.Amount) // There's space for item
                {
                    int itemLeft = item.Amount;
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (0 >= itemLeft - item.MaxAmount - items[tmp[i]].Amount) // Fits
                        {
                            items[tmp[i]].Amount += itemLeft; // Added the remaining of the item
                            return true;
                        }
                        itemLeft -= item.MaxAmount - items[tmp[i]].Amount;
                        items[tmp[i]].Amount += item.MaxAmount - items[tmp[i]].Amount; //

                    }
                }
            }
        }
        return false; // Can't stack or there is no space
    }
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
    public bool RemoveFromSlot(int pos) 
    {
        if (items[pos] != null)
        {
            items[pos] = null;
            return true;
        }
        return false;        
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
    public void Clear() 
    {
        items = new Item[maxSlots];
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = null;
        }
    }
    private LinkedList<int> FindAll(int id) 
    {
        LinkedList<int> sameID = new LinkedList<int>();
        for (int i = 0; i < maxSlots; i++)
        {
            if (items[i].id == id)
            {
                sameID.AddLast(i);
            }
        }
        return sameID;
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
    private LinkedList<int> FindStackables(Item item) // Finding Slots with the same ID that are not full
    {
        LinkedList<int> stackables = new LinkedList<int>();
        if (item.MaxAmount != 1)
        {
            for (int i = 0; i < maxSlots; i++) // Searches through the whole array
            {
                if (items[i] != null) // If the slot is occupied
                {
                    if (items[i].id == item.id && items[i].IsFull != false) // Found identical item 
                    {
                        stackables.AddLast(i);
                    }
                }
            }
        }
        return stackables; // returns a list filled with positions of the same stackable items 
    }


}
