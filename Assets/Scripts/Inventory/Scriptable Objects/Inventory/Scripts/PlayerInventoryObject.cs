using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Inventory", menuName = "Inventory System/Player Inventory")]
public class PlayerInventoryObject : InventoryObject
{
    public InventorySlot[] playerInventory = new InventorySlot[32];
    public int gold;
    public InventoryObject inventoryObject;
    private bool noMoreSlots;

    public bool NoMoreSlots { get => noMoreSlots; set => noMoreSlots = value; }

    public void BeginANew() 
    {
        for (int i = 0; i < playerInventory.Length; i++)
        {
            playerInventory[i] = null;
        }
        noMoreSlots = false;
    }
    public void Awake()
    {
        BeginANew();
        if (inventoryObject.container.Count == playerInventory.Length)
        {
            noMoreSlots = true;
        }
    }
}
