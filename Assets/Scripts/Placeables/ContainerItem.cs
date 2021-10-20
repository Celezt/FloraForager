using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : IItem, IPlaceable, IDestructable, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; }
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    float IDestructable.Durability { get; set; } = 1;
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    GameObject IPlaceable.WorldObject { get; set; }
    [OdinSerialize, PropertyOrder(int.MinValue + 4)]
    List<IResource.DropType> IResource.Drops { get; set; } = new List<IResource.DropType>();

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
