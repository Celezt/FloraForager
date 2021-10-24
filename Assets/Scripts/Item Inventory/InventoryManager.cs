using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
public class InventoryManager : MonoBehaviour, IDropHandler
{
    [SerializeField] PlayerInventory _playerInventory;

    private GraphicRaycaster _raycaster;
    private EventSystem _EventSystem;

    private void Awake()
    {
        _EventSystem = FindObjectOfType<EventSystem>();
        _raycaster =  GetComponent<GraphicRaycaster>();
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        RectTransform uiGrid = transform as RectTransform;

        if (!CanvasUtility.IsPointerOverUIElement())    // If dropping.
        {
            ItemSlot holder = eventData.selectedObject.GetComponentInParent<ItemSlot>();

            if (holder == null)
                return;

            if (string.IsNullOrEmpty(holder.Item.ID))
                return;

            Transform playerTransform = PlayerInput.GetPlayerByIndex(_playerInventory.PlayerIndex).transform;
            
            UnityEngine.Object.Instantiate(ItemTypeSettings.Instance.ItemObject, playerTransform.position, Quaternion.identity)
                    .Spawn(new ItemAsset { ID = holder.Item.ID, Amount = holder.Item.Amount }, playerTransform.forward.xz());

            holder.InventoryHandler.Inventory.RemoveAt(holder.Index);
        }
        else
        {
            // Set up the new Pointer Event.
            PointerEventData PointerEventData = new PointerEventData(_EventSystem);
            // Set the Pointer Event Position to that of the mouse position.
            PointerEventData.position = Mouse.current.position.ReadValue();

            // Create a list of Raycast Results.
            List<RaycastResult> results = new List<RaycastResult>();

            // Raycast using the Graphics Raycaster and mouse click position
            _raycaster.Raycast(PointerEventData, results);

            // For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            // find and swap itemslots.
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
                            Inventory firstInventory = firstHolder.InventoryHandler.Inventory;
                            Inventory secondInventory = secondHolder.InventoryHandler.Inventory;

                            if (!Inventory.Merge(secondHolder.Index, firstHolder.Index, secondInventory, firstInventory)) // If items is not mergeable.
                                Inventory.Swap(firstHolder.Index, secondHolder.Index, firstInventory, secondInventory);
                        }
                    }
                }
            }
        }
    }
}
