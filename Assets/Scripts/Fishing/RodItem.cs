using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class RodItem : IItem, IUse
{
    public float Proficiency => _proficiency;
    public float CatchSize => _catchSize;
    public float DragVelocity => _dragVelocity;
    public float DragAcceleration => _dragAcceleration;
    public float DragWeight => _weight;
    public float Bounciness => _bounciness;

    [OdinSerialize]
    uint IItem.ItemStack { get; set; } = 1;
    [OdinSerialize]
    float IUse.Cooldown { get; set; } = 1;

    [SerializeField, Range(0, 1)]
    private float _catchSize = 0.2f;
    [SerializeField]
    private float _proficiency = 1;
    [SerializeField]
    private float _weight = 6.0f;
    [SerializeField]
    private float _dragVelocity = 100.0f;
    [SerializeField]
    private float _dragAcceleration = 10.0f;
    [SerializeField]
    private float _bounciness = 0.5f;

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
