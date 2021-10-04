using MyBox;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public event Action<int> InventoryAction = delegate { };
    public ItemAsset[] Container = new ItemAsset[8];
    public bool IsFull { get; set; }
    public bool AddItem(ItemAsset item)
    {
        if (!IsFull)
        {
            int pos = ExistsAt(item.ID);
            if (pos != -1)
            {
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
            // Find Stackables
        }
        return false;
    }
    public void Swap(int pos, int pos2)
    {
        ItemAsset holder = Container[pos];
        Container[pos] = Container[pos2];
        Container[pos2] = holder;
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
