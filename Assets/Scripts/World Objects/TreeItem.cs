using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class TreeItem : IItem, IUse, IPlace, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 64;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    Stars IStar.Star { get; set; } = Stars.One;
    [AssetsOnly]
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    GameObject IPlace.PlaceableObject { get; set; }
     
    float IUse.Cooldown { get; set; } = 0;

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

        Object.Instantiate((this as IPlace).PlaceableObject, context.transform.position + context.transform.forward, Quaternion.identity);

        context.Consume();

        yield break;
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }
}
