using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class ChopItem : IItem, IUse, IDestructor
{
    [OdinSerialize]
    uint IItem.ItemStack { get; set; } = 1;
    [OdinSerialize]
    float IUse.Cooldown { get; set; } = 1.0f;
    [OdinSerialize]
    float IDestructor.Strength { get; set; } = DurabilityStrengths.BRITTLE_STONE;
    [OdinSerialize]
    float IDestructor.Damage { get; set; } = 2.0f;

    [SerializeField]
    private Vector3 _halfExtents = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField]
    private Vector3 _centerOffset = new Vector3(0, 0.5f, 0);

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
        if (!context.started)
            yield break;

        Collider[] colliders = Physics.OverlapBox(context.playerTransform.position + _centerOffset, _halfExtents, context.playerTransform.rotation, LayerMask.NameToLayer("default"));
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].TryGetComponent(out IUsable usable))
                yield return usable;
    }
}
