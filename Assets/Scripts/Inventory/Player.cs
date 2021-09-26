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
            var item = other.GetComponent<Item>();
            if (item)
            {
                inventory.AddItem(item.item.item, item.item.amount);
                Destroy(other.gameObject);
            }
        }
    }
    
    public void Update()
    {
        
    }
    private void OnApplicationQuit()
    {
        //inventory.container.Clear();
    }
}
