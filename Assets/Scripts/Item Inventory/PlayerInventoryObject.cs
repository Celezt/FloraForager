using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Inventory", menuName = "Inventory System/Player Inventory")]
public class PlayerInventoryObject : InventoryObject
{
    public int gold;
    public InventoryObject inventory;
    public ItemSlot currentSlot;
    public void UsingHotbarSlot(int pos)
    {
        currentSlot.item = inventory.Container[pos];
    }
}
