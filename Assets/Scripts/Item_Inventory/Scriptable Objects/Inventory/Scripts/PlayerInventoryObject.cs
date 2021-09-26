using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Inventory", menuName = "Inventory System/Player Inventory")]
public class PlayerInventoryObject : InventoryObject
{
    public int gold;
    public InventoryObject inventory;
    public Item currentSlot;
    public void UsingHotbarSlot(int pos)
    {
        currentSlot.item = inventory.Container[pos];        
    }
    public void BeginANew() 
    {
        for (int i = 0; i < inventory.Container.Length; i++)
        {
            inventory.Container[i] = null;
        }
        inventory.IsFull = false;
    }
}
