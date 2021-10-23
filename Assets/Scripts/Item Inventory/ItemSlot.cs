using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public InventoryHandler InventoryHandler { get; set; }
    public int Index { get; set; }
    public ItemAsset Item { get; set; }
    public string Name { get; set; }
    public TextMeshProUGUI Amount => _amount;
    public Image Icon => _icon;
    public Image Background => _background;
    public RectTransform FrameTransform => _frameTransform;

    [SerializeField]
    private TextMeshProUGUI _amount;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private Image _background;
    [SerializeField]
    private RectTransform _frameTransform;

    public void OnSelect(BaseEventData eventData)
    {
        if (InventoryHandler.IsItemSelectable)                      // If items inside of the handler is selectable.
            if (InventoryHandler.Inventory.SelectedIndex != Index)  // If not already selected.
                InventoryHandler.Inventory.SetSelectedItem(Index);
    }
}