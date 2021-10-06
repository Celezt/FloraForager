using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IDropHandler
{
    private ItemSlot[] slots;
    public InventoryObject inventory;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    ItemTypeSettings settings;

    void Start()
    {
        inventory.InventoryAction += (int i) =>
        {
            if (!(i >= slots.Length))
            {
                slots[i].item = inventory.Container[i];
                if (slots[i].item.ID != null)
                {
                    if (settings.ItemIconChunk.TryGetValue(slots[i].item.ID, out Sprite sprite))
                    {
                        slots[i].image.sprite = sprite;
                    }
                    else
                    {
                        slots[i].image.sprite = null;
                    }
                }
                else
                {
                    slots[i].image.sprite = null;
                }
                
                slots[i].TextMesh.text = inventory.Container[i].Amount.ToString();
            }
            
            //Debug.Log(inventory.Container[i].Amount.ToString());
        };
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        Addressables.LoadAssetAsync<TextAsset>("inventory").Completed +=(handle)=>
        {
            settings = ItemTypeSettings.Instance;
            inventory.Container = new ItemAsset[32];
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            for (int i = 0; i < tmp.Items.Length; i++)
            {
                inventory.Container[i] = tmp.Items[i];
            }
            slots = GetComponentsInChildren<ItemSlot>();

            // Assigns Items to slots
            for (int i = 0; i < slots.Length; i++) // Assigns Items to slots
            {
                slots[i].pos = i;
                if (inventory.Container[i].ID != null)
                {
                    slots[i].item = inventory.Container[i];
                    if (settings.ItemIconChunk.TryGetValue(slots[i].item.ID, out Sprite sprite))
                    {
                        slots[i].image.sprite = sprite;
                    }
                    slots[i].TextMesh.text = inventory.Container[i].Amount.ToString(); // Can use slot instead?
                    //Debug.Log(inventory.Container[i].Amount);
                }
            }
            //inventory.AddItem(new ItemAsset { ID = "Loka", Amount = 10});
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
            // Drop slot into overworld
            // Empty slot
        }
        else
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Mouse.current.position.ReadValue();

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            //Find and swap itemslots
            foreach (RaycastResult result in results)
            {
                //Debug.Log("Hit " + result.gameObject.name);
                ItemSlot holder = result.gameObject.GetComponent<ItemSlot>();
                if (holder != null)
                {
                    ItemSlot holder2 = eventData.selectedObject.GetComponentInParent<ItemSlot>();
                    if (holder2 != null)
                    {
                        if (holder.pos != holder2.pos)
                        {
                            inventory.Swap(holder.pos,holder2.pos);
                        }
                    }
                }
            }
        }
    }
}
