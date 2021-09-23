using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Inventory", menuName = "Inventory System/Player Inventory")]
public class PlayerInventoryObject : InventoryObject
{
    public InventorySlot[] playerInventory = new InventorySlot[32];
    public int gold;
    public InventoryObject inventoryObject;
    private bool isFull;

    public bool IsFull { get => isFull; set => isFull = value; }

    public void BeginANew() 
    {
        for (int i = 0; i < playerInventory.Length; i++)
        {
            playerInventory[i] = null;
        }
        isFull = false;
    }
    public void Awake()
    {
        BeginANew();
        if (inventoryObject.container.Count == playerInventory.Length)
        {
            isFull = true;
        }
    }
}
