using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class FishBait : IItem, IBait
{
    [OdinSerialize]
    int IItem.ItemStack { get; set; } = 64;
    [OdinSerialize]
    float IBait.Efficiency { get; set; } = 5;

    void IItem.OnEquip(ItemContext context)
    {
        
    }

    void IItem.OnUnequip(ItemContext context)
    {

    }

    void IItem.OnUpdate(ItemContext context)
    {

    }
}
