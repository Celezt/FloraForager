using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public struct ScytheItem : IUse, IItem
{
    [OdinSerialize]
    public uint ItemStack { get; set; }

    public void OnActive(UseContext context)
    {

    }

    public void OnInactive(UseContext context)
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
