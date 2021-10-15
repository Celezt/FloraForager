using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IDropHandler
{
    private GraphicRaycaster _raycaster;
    private EventSystem _EventSystem;
    private ItemTypeSettings _settings;

    private void Awake()
    {
        _EventSystem = GetComponent<EventSystem>();
        _raycaster = GetComponentInParent<GraphicRaycaster>();
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
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
            PointerEventData PointerEventData = new PointerEventData(_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            PointerEventData.position = Mouse.current.position.ReadValue();

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            _raycaster.Raycast(PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            //Find and swap itemslots
            foreach (RaycastResult result in results)
            {
                ItemSlot firstHolder = result.gameObject.GetComponent<ItemSlot>();
                if (firstHolder != null)
                {
                    ItemSlot secondHolder = eventData.selectedObject.GetComponentInParent<ItemSlot>();
                    if (secondHolder != null)
                    {
                        if (firstHolder.Index != secondHolder.Index)
                        {
                            Inventory firstInventory = firstHolder.InventoryManager.Inventory;
                            Inventory secondInventory = secondHolder.InventoryManager.Inventory;

                            ItemAsset holder = firstInventory.Swap(firstHolder.Index, secondInventory.Get(secondHolder.Index));
                            secondInventory.Swap(secondHolder.Index, holder);
                        }
                    }
                }
            }
        }
    }
}
