using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class TreeItem : IItem, IUse, IPlaceable, IResource, IDestructable, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 64;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    float IDestructable.Durability { get; set; } = 5;
    [AssetsOnly]
    [OdinSerialize, PropertyOrder(int.MinValue + 4)]
    GameObject IPlaceable.WorldObject { get; set; }

    float IUse.Cooldown { get; set; } = 0;

    [Title("Resource Behaviour")]
    [OdinSerialize, PropertyOrder(int.MinValue + 5)]
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

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Object.Instantiate((this as IPlaceable).WorldObject, context.transform.position + context.transform.forward, Quaternion.identity);

        context.Consume();

        yield break;
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }
}
