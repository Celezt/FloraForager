using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour, IStreamable<TreeBehaviour.Data>, IUsable
{
    [SerializeField] private ItemLabels _filter = ItemLabels.Axe;
    [SerializeField] private Stars _star = Stars.One;
    [SerializeField] private float _durability = 5;
    [SerializeField] private List<DropType> _drops = new List<DropType>();

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
    ItemLabels IUsable.Filter() => _filter;

    void IUsable.OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor))
            return;

        int usedStar = (int)Stars.One;
        int star = (int)_star;

        if (context.used is IStar)
            usedStar = (int)(context.used as IStar).Star;

        if (usedStar >= star)
        {

        }
    }
}
