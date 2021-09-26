using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Inventory", menuName = "Inventory System/Player Inventory")]
public class PlayerInventoryObject : InventoryObject
{
    public InventorySlot[] inventoryArray = new InventorySlot[32];
    public int gold;
    public InventoryObject inventoryObject;
    private bool isFull;
    public ItemObject currentSlot;

    public bool IsFull { get => isFull; set => isFull = value; }
    public void UsingHotbarSlot(int pos)
    {
        currentSlot = inventoryObject.container[pos].item;        
    }
    public void BeginANew() 
    {
        for (int i = 0; i < inventoryArray.Length; i++)
        {
            inventoryArray[i] = null;
        }
        isFull = false;
    }
    public void Awake()
    {
        BeginANew();
        if (inventoryObject.container.Count == inventoryArray.Length)
        {
            isFull = true;
        }
    }
}
