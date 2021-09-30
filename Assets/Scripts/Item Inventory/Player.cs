using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInventoryObject inventory;
    
    public void OnTriggerEnter(Collider other)
    {
        if (!inventory.IsFull) // Inventory is not full
        {
            var item = other.GetComponent<ItemSlot>();
            if (item)
            {
                inventory.AddItem(item.item);
                Destroy(other.gameObject);
            }
        }
    }
    


}
