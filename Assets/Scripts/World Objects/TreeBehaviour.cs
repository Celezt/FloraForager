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

    [SerializeField]
    ItemLabels IUsable.Filter() => _filter;

    void IUsable.OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor))
            return;

        IDestructor destructor = context.used as IDestructor;
        int usedStar = (int)Stars.One;
        int star = (int)_star;

        if (context.used is IStar)
            usedStar = (int)(context.used as IStar).Star;

        if (usedStar + 1 >= star)   // Can damage if at least one star below.
            _durability = Mathf.Clamp(_durability - destructor.Damage * cmath.Map(usedStar / star, new MinMaxFloat(1, 5), new MinMaxFloat(1, 2)), 0, float.MaxValue);

        if (_durability <= 0)
        {
            ItemTypeSettings settings = ItemTypeSettings.Instance;

            for (int i = 0; i < _drops.Count; i++)
            {
                int rate = _drops[i].DropRate.RandomInRangeInclusive();
                //_drops[i].Item
                for (int j = 0; j < rate; j++)
                {
                    Instantiate(settings.ItemObject, transform.position, Quaternion.identity).Initialize(settings.DefaultIcon.texture);
                }
            }

            Destroy(gameObject);
        }
    }
}
