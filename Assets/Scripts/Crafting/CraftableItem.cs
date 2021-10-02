using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftableItem
{
    private CraftableItemData _Data;

    public string ItemID => _Data.ItemID;
    public Sprite Sprite => _Data.Sprite;
    public ResourceRequirement<string, int>[] ResourceReqs => _Data.ResourceReqs;

    public UICraftableItemObject Object { get; set; }

    public CraftableItem(CraftableItemData data)
    {
        _Data = data;
    }
}
