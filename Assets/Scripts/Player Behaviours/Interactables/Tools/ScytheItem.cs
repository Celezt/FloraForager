using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public struct ScytheItem : IUse, IItem
{
    int s;
    [OdinSerialize]
    public uint ItemStack { get; set; }

    public void OnEquip(UseContext context)
    {

    }

    public void OnUnequip(UseContext context)
    {

    }

    public void OnUse(UseContext context)
    {
        Debug.Log(s++);
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
