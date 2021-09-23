using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInventoryObject inventory;
    public void OnTriggerEnter(Collider other)
    {
        if (!inventory.NoMoreSlots)
        {
            var item = other.GetComponent<Item>();
            if (item)
            {
                inventory.AddItem(item.item, 1);
                Destroy(other.gameObject);
            }
        }
    }
    private void OnApplicationQuit()
    {
        //inventory.container.Clear();
    }
}
