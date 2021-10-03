using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour, IDropHandler
{
    private ItemSlot[] slots;
    public InventoryObject inventory;
    private ItemSlot selectedSlot;
    // Start is called before the first frame update
    void Start()
    {
        inventory.InventoryAction += (int i) =>
        {
            slots[i].item = inventory.Container[i];
            slots[i].TextMesh.text = inventory.Container[i].Amount.ToString();
            //Debug.Log(inventory.Container[i].Amount.ToString());
        };

        Addressables.LoadAssetAsync<TextAsset>("inventory").Completed +=(handle)=>
        {
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            for (int i = 0; i < tmp.Items.Length; i++)
            {
                inventory.Container[i] = tmp.Items[i];
            }
            slots = GetComponentsInChildren<ItemSlot>();
            for (int i = 0; i < inventory.Container.Length; i++)
            {
                slots[i].item = inventory.Container[i];
            }
            
            for (int i = 0; i < 8; i++)
            {
                if (inventory.Container[i].ID != null)
                {
                    slots[i].item = inventory.Container[i];
                    slots[i].TextMesh.text = inventory.Container[i].Amount.ToString();
                    Debug.Log(inventory.Container[i].Amount);
                }
            }
            //inventory.AddItem(new ItemAsset { ID = "Loka", Amount = 10});
            selectedSlot = slots[0];
            Addressables.Release(handle);
        };
        //selectedSlot = slots[0];
    }

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform uiGrid = transform as RectTransform;
        
        if (!RectTransformUtility.RectangleContainsScreenPoint(uiGrid, Mouse.current.position.ReadValue())) // Outside of the ui grid
        {
            Debug.Log("Dropped outside");
        }
        else
        {
            
            for (int i = 0; i < slots.Length; i++)
            {
                
                RectTransform holder = slots[i].GetComponent<RectTransform>();


                /*if (slots[i].GetComponent<RectTransform>()..Contains(Mouse.current.position.ReadValue()))
                {
                    Debug.Log("Dropped into slot");
                }*/
            }
            Debug.Log("Dropped inside");
        }
    }
}
