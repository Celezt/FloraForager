using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenuAttribute(fileName = "New Food Object", menuName = "Inventory/Items/Food")]
public class FoodObject : ItemObject
{
    public int restoreEnergyValue;
    public void Awake()
    {
        type = ItemType.Food;
    }
}
