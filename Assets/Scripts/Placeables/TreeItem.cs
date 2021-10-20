using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class TreeItem : IItem, IPlaceable, IResource, IDestructable
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 16;
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    float IDestructable.Strength { get; set; } = 1;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    float IDestructable.Durability { get; set; } = 5;
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    [AssetsOnly]
    GameObject IPlaceable.WorldObject { get; set; }
    [OdinSerialize, PropertyOrder(int.MinValue + 4)]
    List<IResource.DropType> IResource.Drops { get; set; } = new List<IResource.DropType>();

    [Title("Resource Behaviour")]


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
