using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class PlaceableItem : IItem, IUse, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 64;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    Stars IStar.Star { get; set; } = Stars.One;

    [SerializeField]
    private LayerMask _placeMask = LayerMask.GetMask("Ground");
    [SerializeField]
    private bool _followNormal;
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

        if (context.Place(_placeableObject, context.transform.position + context.transform.forward, Quaternion.identity, _placeMask, _followNormal))
            context.Consume();

        yield break;
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }
}
