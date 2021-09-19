using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int maxAmount;
    public int amount;
    public bool isFull;
    public bool stackable;
    public int id;
    public string itemName;
    public Item(Item item) 
    {
    maxAmount = item.maxAmount;
    amount = item.amount;
    isFull = item.isFull;
    stackable = item.stackable;
    id = item.id;
    itemName = item.itemName;
    }
}
