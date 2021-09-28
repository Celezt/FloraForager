using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Item[] slots;
    public InventoryObject inventory;

    // Start is called before the first frame update
    void Start()
    {
        slots = GetComponentsInChildren<Item>();
        for (int i = 0; i < inventory.Container.Length; i++)
        {
            slots[i].item = inventory.Container[i];
        }
        inventory.InventoryAction += (int i)=> 
        { 
            slots[i].item = inventory.Container[i];
            slots[i].TextMesh.text = inventory.Container[i].item.Amount.ToString();
        
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
