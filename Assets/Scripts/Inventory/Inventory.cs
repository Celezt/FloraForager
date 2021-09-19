using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private int maxSlots;
    private int nrItems;
    public int gold;
    public Item[] items;
    public bool isFull;

    public void IniFromFile()
    {

    }
    public void Initiate()
    {

    }
    public bool AddItem(Item item)
    {
        if (!isFull) // Not full, can add
        {
            if (item.stackable && Exists(item.id)) // Is stackable and already occupies a slot
            {
                if (Stacking(item))
                {
                    return true;
                }
                else // Not full and does not already exist 
                {
                    items[FindFirstEmptySlot()] = new Item(item);
                    return true;
                }
            }
            else // Does not Exist in a slot, Add to an empty slot
            {
                items[FindFirstEmptySlot()] = new Item(item);
                return true;
            }
        }
        else if (!item.isFull) // Inventory slots full, but slots might be stackable
        {
            if (Exists(item.id)) // If item exists
            {

            }
            else
            {
                return false;
            }


            int count = 0;
            bool foundSlot = false;
            while (count < maxSlots || foundSlot)
            {
                if (items[count].id == item.id && items[count].amount + item.amount <= items[count].maxAmount)
                {
                    items[count].amount += item.amount;
                }
            }
        }        
        Debug.Log("Inventory full");
        return false;
    }
    private LinkedList<int> FindStackables(int id) // Finding Slots with the same ID that are not full
    {
        LinkedList<int> stackables = new LinkedList<int>();
        for (int i = 0; i < maxSlots; i++)
        {
            if (items[i].id == id && items[i].isFull != false)
            {

            }
        }
        return stackables; // returns 
    }

    private bool Stacking(Item item)  // Tries to stack the item
    {
        LinkedList<int> stackables = FindStackables(item.id);
        int amountLeft = item.amount;
        int maxAmount = item.maxAmount;
        if (stackables != null)
        {
            for (int i = 0; i < stackables.Count; i++)
            {
                if (0 >  maxAmount - items[stackables.First.Value].amount - amountLeft) // Fits
                {
                    
                }
                else
                {

                    return true; // Stacked successfully
                }
                stackables.RemoveFirst();
            }
        }
        return false; // Can't stack
    }
    
    private bool Exists(int id) 
    {
        bool found = false;
        for (int i = 0; i < maxSlots; i++)
        {
            if (items[i].id == id)
            {
                return found;
            }
        }
        
        return found;
    }
    private int FindFirstEmptySlot() 
    {
        if (!isFull)
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (items[i] == null)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public void RemoveItem() 
    {
    
    }
    
}
