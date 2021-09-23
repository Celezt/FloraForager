using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType 
{
Food,
Equipment,
Default,
Tool,
Seed
}
public abstract class ItemObject : ScriptableObject
{
    public GameObject prefabOverworld;
    public GameObject prefabInventorySlot;
    public int maxAmount;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
}
