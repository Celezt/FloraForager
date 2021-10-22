using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour, IStreamableObject<TreeBehaviour.Data>, IUsable
{
    private Data _data;

    [SerializeField]
    [AssetList(CustomFilterMethod = nameof(HasIPlace))]
    private ItemType _item;

    public bool HasIPlace(ItemType itemType) => itemType.Behaviour is IPlace;


    private float _durability;

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
}
