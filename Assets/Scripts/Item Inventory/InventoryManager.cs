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
    private ItemSlot selectedSlot;
    private RectTransform rectTransforms;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    // Start is called before the first frame update
    void Start()
    {
        inventory.InventoryAction += (int i) =>
        {
            slots[i].item = inventory.Container[i];
            slots[i].TextMesh.text = inventory.Container[i].Amount.ToString();
            //Debug.Log(inventory.Container[i].Amount.ToString());
        };
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        Addressables.LoadAssetAsync<TextAsset>("inventory").Completed +=(handle)=>
        {
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            for (int i = 0; i < tmp.Items.Length; i++)
            {
                inventory.Container[i] = tmp.Items[i];
            }
            slots = GetComponentsInChildren<ItemSlot>();
            for (int i = 0; i < inventory.Container.Length; i++) // Change this to slots.length // Da faq does this do?
            {
                slots[i].item = inventory.Container[i];
                slots[i].pos = i;
            }
            
            for (int i = 0; i < 8; i++) // Change this // Assigning Items
            {
                if (inventory.Container[i].ID != null)
                {
                    slots[i].item = inventory.Container[i];
                    
                    slots[i].TextMesh.text = inventory.Container[i].Amount.ToString(); // Can use slot instead?
                    //Debug.Log(inventory.Container[i].Amount);
                }
            }
            //inventory.AddItem(new ItemAsset { ID = "Loka", Amount = 10});
            selectedSlot = slots[0];
            Addressables.Release(handle);
        };
        //selectedSlot = slots[0];
    }
    public void Update()
    {
        
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
            int counter = 0;
            
            foreach (RaycastResult result in results)
            {
                Debug.Log("Hit " + result.gameObject.name);
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
            Debug.Log(counter.ToString());

            // Find the slot it is being dragged into
            // Swap with the dragged item's slot
            

            Debug.Log("Dropped inside");
        }
    }
}
