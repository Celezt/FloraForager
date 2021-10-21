using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour, IStreamableObject<TreeData>, IUsable, IResourceObject, IDestructableObject
{
    private TreeData _data = new TreeData();

    TreeData IStreamableObject<TreeData>.Data() => _data;

    [SerializeField]
    ItemLabels IUsable.Filter() => ItemLabels.Axe;

    void IUsable.OnUse(UsedContext context)
    {

    }

    void IDestructableObject.OnDamage(IDestructor destructor, IDestructable destructable, UsedContext context)
    {

    }

    void IDestructableObject.OnDestruction(UsedContext context)
    {

    }
}
