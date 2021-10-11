using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotButton : MonoBehaviour, ISelectHandler
{
    public InventoryManager inventoryManager;
    public ItemSlot itemSlot;
    public void OnSelect(BaseEventData eventData)
    {
        inventoryManager.SelectItem(itemSlot);
        //Debug.Log("Selected Item is " + itemSlot.item.ID);
    }

   
}
