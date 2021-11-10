using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICraftableItemObject : MonoBehaviour
{
    [SerializeField] private Image _Image;

    private float _NormalAlpha;

    public CraftableItem CraftableItem { get; set; }

    private void Awake()
    {
        _NormalAlpha = _Image.color.a;
    }

    private void Start()
    {
        _Image.sprite = ItemTypeSettings.Instance.ItemIconChunk[CraftableItem.Item.ID];
    }

    public void Select()
    {
        Color color = _Image.color;
        color.a = 1.0f;
        _Image.color = color;

        UICraftingMenu.Instance.ShowDescription(CraftableItem);
    }

    public void Deselect()
    {
        Color color = _Image.color;
        color.a = _NormalAlpha;
        _Image.color = color;
    }
}
