using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class ScytheItem : IUse, IItem, IDestructor
{
    [OdinSerialize]
    public uint ItemStack { get; set; } = 1;
    [OdinSerialize]
    public float Strength { get; set; } = DurabilityStrengths.BRITTLE_STONE;
    [OdinSerialize]
    public float Damage { get; set; } = 2.0f;

    public void OnEquip(UseContext context)
    {

    }

    public void OnUnequip(UseContext context)
    {

    }

    public void OnUse(UseContext context)
    {
        if (context.started)
        {
            Collider[] colliders = PhysicsC.OverlapArc(context.playerTransform.position, context.playerTransform.forward, Vector3.up, 4, 0.5f, LayerMask.NameToLayer("default"));
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out IUsable usable))
                {
                    usable.OnUse(context);
                }
            }
        }
    }

    public void OnUpdate(UseContext context)
    {

    }
}
