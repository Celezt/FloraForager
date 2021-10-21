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

    public Data OnUpload() => _data = new Data();
    public void OnLoad(object state)
    {
        Data data = state as Data;
        _data = data;
    }


    [SerializeField]
    ItemLabels IUsable.Filter() => ItemLabels.Axe;

    void IUsable.OnUse(UsedContext context)
    {
        _data.Hej++;
    }

    void IDestructableObject.OnDamage(IDestructor destructor, IDestructable destructable, UsedContext context)
    {

    }

    void IDestructableObject.OnDestruction(UsedContext context)
    {

    }
}
