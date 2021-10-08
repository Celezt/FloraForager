using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : MonoBehaviour
{
    public event Action<int> InventoryAction = delegate { };
    public ItemAsset[] Container; // Change
    public ItemSlot currentSlot;
    public int gold;
    public bool IsFull { get; set; }
    public bool AddItem(ItemAsset item)
    {
        if (!IsFull)
        {
            int pos = ExistsAt(item.ID);
            if (pos != -1) // It exists
            {
                // Check if new amount > max amount
                Container[pos].Amount += item.Amount;
                InventoryAction.Invoke(pos);
            }
            else
            {
                int tmp = FindFirstEmptySlot();
                Container[tmp] = item;
                InventoryAction.Invoke(tmp);
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
        if (pos < Container.Length)
        {
            Container[pos] = new ItemAsset();
            InventoryAction.Invoke(pos);
            return true;
        }
        return false;
    }
    public void Swap(int pos, int pos2)
    {
        ItemAsset holder = Container[pos];
        Container[pos] = Container[pos2];
        Container[pos2] = holder;
        InventoryAction.Invoke(pos);
        InventoryAction.Invoke(pos2);
    }
    public int ExistsAt(string id) 
    {
        for (int i = 0; i < Container.Length; i++)
        {
            if (!Container[i].ID.IsNullOrEmpty())
            {
                if (Container[i].ID == id)
                {
                    return i;
                }
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
