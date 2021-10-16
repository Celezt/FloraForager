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

    [SerializeField]
    private TextMeshProUGUI _amount;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private Image _icon;

    public void OnSelect(BaseEventData eventData)
    {
        InventoryHandler.Inventory.SetSelectedItem(Index, Item);
    }
}