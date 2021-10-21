using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour, IStreamableObject<TreeBehaviour.Data>, IUsable, IResourceObject, IDestructableObject
{
    private Data _data;

    public class Data
    {
        public int Hej = 10;
    }

    public Data OnUnload() => _data = new Data();
    public void OnLoad(object state) => _data = state as Data;


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
