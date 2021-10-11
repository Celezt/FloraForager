using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class RodItem : IItem, IUse
{
    public float Proficiency => _proficiency;
    public float CatchSize => _catchSize;

    [OdinSerialize]
    uint IItem.ItemStack { get; set; } = 1;
    [OdinSerialize]
    float IUse.Cooldown { get; set; } = 1;

    [SerializeField, Min(0)]
    private float _proficiency = 1;
    [SerializeField, Range(0, 1)]
    private float _catchSize = 0.2f;

    void IItem.Initialize(ItemContext context)
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

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {

        yield break;
    }
}
