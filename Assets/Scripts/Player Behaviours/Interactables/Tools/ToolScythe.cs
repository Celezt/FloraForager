using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolScythe : IUse
{
    public IEnumerable<IUsable> OnUse(UseContext context)
    {
        if (context.started)
        {
            Collider[] colliders = PhysicsC.OverlapArc(context.playerTransform.position, context.playerTransform.forward, Vector3.up, 2, 0.5f, LayerMask.NameToLayer("default"));
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent(out IUsable usable))
                {
                    yield return usable;
                }
            }
        }
    }
}
