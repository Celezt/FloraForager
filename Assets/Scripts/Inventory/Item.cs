using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int maxAmount;
    private int amount;
    private bool isFull;
    public int id;
    public string itemName;

    public Item(int id,string itemName, int maxAmount, int amount) 
    {
        this.id = id;
        this.itemName = itemName;
        MaxAmount = maxAmount;
        Amount = amount;
    }

    public int Amount 
    {
        get { return amount; } 
        set 
        {
            if (value <= 0) // Value lower or equal to zero 
            {
                amount = 0;
                isFull = false;
            }
            else if (value >= MaxAmount) // Value is higher or equal to maxAmount
            {
                amount = MaxAmount; // Amount must never exceed maxAmount - Set to maxAmount instead of value
                isFull = true; // 
            }
            else
            {
                amount = value;
            }
        }
    }

    public bool IsFull { get => isFull; }
    public int MaxAmount 
    {
        get { return maxAmount; }
        set
        {
            if (value <= 1)
            {
                isFull = true;
            }
            else
            {
                isFull = false;
            }
        }
    }
    public Item(Item item) 
    {
        id = item.id;
        MaxAmount = item.MaxAmount;
        Amount = item.Amount;    
        itemName = item.itemName;
    }
}
