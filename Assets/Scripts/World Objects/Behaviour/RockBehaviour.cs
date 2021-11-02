using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour, IStreamable<RockBehaviour.Data>, IUsable
{
    [SerializeField] private ItemLabels _filter = ItemLabels.Pickaxe;
    [SerializeField] private Stars _star = Stars.One;
    [SerializeField] private float _durability = 20;
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
            context.Drop(_drops);

            Destroy(gameObject);
        }
    }
}
