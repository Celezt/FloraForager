using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftableItem
{
    private CraftableItemInfo _Info;

    public string ItemID => _Info.ItemID;
    public ItemAsset[] Requirements => _Info.Requirements;

    public UICraftableItemObject Object { get; set; }

    public CraftableItem(CraftableItemInfo info)
    {
        _Info = info;
    }
}
