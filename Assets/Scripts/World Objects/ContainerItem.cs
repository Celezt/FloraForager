using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : IItem, IPlace, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 16;
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    GameObject IPlace.PlaceableObject { get; set; }

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

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
