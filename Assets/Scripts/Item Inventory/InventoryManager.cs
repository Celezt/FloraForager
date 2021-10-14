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
    public string inventoryID;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    ItemTypeSettings settings;
    public ItemSlot currentSlot;

    public void SelectItem(ItemSlot itemSlot) 
    {
        currentSlot = itemSlot;
        inventory.SetSelectedItem(itemSlot.item);
        //Debug.Log("Selected Item is " + currentSlot.item.ID);
    }

    void Start()
    {
        inventory.OnItemChangeCallback += (int i) =>
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


        // This occurs 2 time for hud and player inv!!!
        Addressables.LoadAssetAsync<TextAsset>(inventoryID).Completed +=(handle)=>
        {
            
            settings = ItemTypeSettings.Instance;
            
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            slots = GetComponentsInChildren<ItemSlot>();
            
            // Assigns Items to slots
            for (int i = 0; i < slots.Length; i++) // Assigns Items to slots
            {                
                inventory.Container.Add(tmp.Items.Length > i ? tmp.Items[i]: new ItemAsset());
                slots[i].pos = i;
                slots[i].GetComponent<ItemSlotButton>().inventoryManager = this;
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
