using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> container = new List<InventorySlot>();

    public void AddItem(ItemObject _item, int _amount)
    {
        bool hasItem = false;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].item = _item)
            {
                container[i].AddAmount(_amount);
                
                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            container.Add(new InventorySlot(_item, _amount));
        }
    }
    public void AddItemAtPos(ItemObject _item, int _amount, int _pos) 
    {
        bool hasItem = false;
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].item = _item)
            {
                container[i].AddAmount(_amount);

                hasItem = true;
                break;
            }
        }
        if (!hasItem)
        {
            container.Add(new InventorySlot(_item, _amount));
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;
    public int pos;
    
    private bool isFull;
    public InventorySlot(ItemObject _item, int _amount)
    {
        item = _item;
        AddAmount(_amount);
        pos = -1;
    }

    public bool IsFull { get => isFull;}
    
    public bool AddAmount(int value)
    {
        bool added = false;
        if (amount + value < item.maxAmount) // lower than max value
        {
            amount += value;
            added = true;
        }
        else if (amount + value == item.maxAmount) // Equals max value
        {
            amount += value;
            isFull = true;
            added = true;
        }
        return added; // Over max value - No Add        
    }
}
