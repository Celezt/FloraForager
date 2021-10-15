using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public InventoryHandler InventoryManager { get; set; }
    public int Index { get; set; }
    public ItemAsset Item { get; set; }

    public TextMeshProUGUI TextMesh;
    public Image Icon;

    public void OnSelect(BaseEventData eventData)
    {
        InventoryManager.SelectItem(this);
    }
}