using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PhysicsC
{
    /// <summary>
    /// Overlap inside of an arc [1, -1].
    /// </summary>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    /// <param name="radius"></param>
    /// <param name="arc"></param>
    /// <param name="layerMask"></param>
    public static Collider[] OverlapArc(Vector3 position, Vector3 direction, Vector3 up, float radius, float arc, LayerMask layerMask)
    {
        Collider[] catchedColliders = Physics.OverlapSphere(position, radius, layerMask);
        int[] indexInsideArc = new int[catchedColliders.Length];
        int length = 0;

        for (int i = 0; i < catchedColliders.Length; i++)
        {
            Vector3 vectorToCollider = Vector3.ProjectOnPlane(catchedColliders[i].transform.position - position, up).normalized;
 
            if (Vector3.Dot(vectorToCollider, direction) > arc)
                indexInsideArc[length++] = i;
        }

        Collider[] collidersInsideArc = new Collider[length];

        for (int i = 0; i < length; i++)
            collidersInsideArc[i] = catchedColliders[indexInsideArc[i]];

        return collidersInsideArc;
    }
}
