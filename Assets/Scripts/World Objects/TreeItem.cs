using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class TreeItem : IItem, IUse, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 64;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    Stars IStar.Star { get; set; } = Stars.One;

    [SerializeField, AssetsOnly]
    private GameObject _placeableObject;
     
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

        context.Place(_placeableObject, context.transform.position + context.transform.forward, Quaternion.identity);
        context.Consume();

        yield break;
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }
}
