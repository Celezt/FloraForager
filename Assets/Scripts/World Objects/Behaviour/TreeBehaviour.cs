using System.Linq;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Mathematics;
using MyBox;

public class TreeBehaviour : MonoBehaviour, IStreamable<TreeBehaviour.Data>, IUsable
{
    [SerializeField] private ItemLabels _filter = ItemLabels.Axe;
    [SerializeField] private Stars _star = Stars.One;
    [SerializeField] private float _durability = 10;
    [SerializeField] private List<DropType> _drops = new List<DropType>();

    private Data _data;

    public class Data
    {

    }

    public Data OnUpload() => _data = new Data();
    public void OnLoad(object state)
    {
        Data data = state as Data;
        _data = data;
    }

    void IStreamable.OnBeforeSaving()
    {

    }

    [SerializeField]
    ItemLabels IUsable.Filter() => _filter;

    void IUsable.OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor))
            return;

        context.Damage(ref _durability, new MinMaxFloat(1, 2), _star);

        if (_durability <= 0)
        {
            context.Drop(transform.position, _drops);
            Destroy(gameObject);
        }
    }
}
