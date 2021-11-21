using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftableItem
{
    private CraftableItemInfo _Info;

    public ItemAsset Item => _Info.Item;
    public float StaminaChange => _Info.StaminaChange;
    public ItemAsset[] Requirements => _Info.Requirements;

    public UICraftableItemObject Object { get; set; }

    public CraftableItem(CraftableItemInfo info)
    {
        _Info = info;
    }
}
