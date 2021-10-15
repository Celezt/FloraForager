using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotButton : MonoBehaviour, ISelectHandler
{
    public InventoryHandler inventoryManager;
    public ItemSlot itemSlot;
    public void OnSelect(BaseEventData eventData)
    {
        inventoryManager.SelectItem(itemSlot);
        //GetComponent<Button>().Select();
        //Debug.Log("Selected Item is " + itemSlot.item.ID);
    }   
}
