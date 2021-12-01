using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RockBehaviour : MonoBehaviour, IStreamable<RockBehaviour.Data>, IUsable
{
    [SerializeField] private Transform _shakeTransform;
    [SerializeField] private string _hitSound = "hit_rock";
    [SerializeField] private string _breakSound = "break_wood";
    [SerializeField] private ItemLabels _filter = ItemLabels.Pickaxe;
    [SerializeField] private Stars _star = Stars.One;
    [SerializeField] private List<DropType> _drops = new List<DropType>();

    [SerializeField, PropertyOrder(-1), HideLabel, InlineProperty]
    private Data _data;

    private BoxCollider _Collider;

    [System.Serializable]
    public class Data
    {
        public float Durability = 10;
    }

    public Data OnUpload() => _data;
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

    private void Start()
    {
        _Collider = GetComponent<BoxCollider>();
    }

    void IUsable.OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor))
            return;

        float previousDurability = _data.Durability;
        context.Damage(ref _data.Durability, new MinMaxFloat(1, 2), _star);

        if (_shakeTransform != null)
            context.Shake(_shakeTransform, 2, angleRotation: 0);

        if (_data.Durability >= previousDurability)
            SoundPlayer.Instance.Play("hit_poor");
        else
            SoundPlayer.Instance.Play(_hitSound);

        if (_data.Durability <= 0)
        {
            SoundPlayer.Instance.Play(_breakSound);

            context.Drop(transform.TransformPoint(_Collider.center), _drops);
            gameObject.SetActive(false);
        }
    }
}
