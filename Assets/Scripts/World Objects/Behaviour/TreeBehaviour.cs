using System.Linq;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Mathematics;
using MyBox;

public class TreeBehaviour : MonoBehaviour, IStreamable<TreeBehaviour.Data>, IUsable
{
    [Header("Properties")]
    [SerializeField] private string _hitSound = "hit_wood";
    [SerializeField] private string _breakSound = "break_wood";
    [SerializeField] private ItemLabels _filter = ItemLabels.Axe;
    [SerializeField] private Stars _star = Stars.One;
    [SerializeField] private List<DropType> _drops = new List<DropType>();
    [Header("Shake Settings")]
    [SerializeField] private Transform _shakeTransform;
    [SerializeField] private float _shakeDuration = 2.0f;
    [SerializeField] private float _shakeStrength = 0.05f;
    [SerializeField] private float _shakeAngleRotation = 1.0f;
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystem _particleSystemLeaf;
    [SerializeField] private int _woodAmount = 0;
    [SerializeField] private int _leafAmount = 0;
    [Header("Objects")]
    [SerializeField] private GameObject _Tree;
    [SerializeField] private GameObject _Stump;

    [SerializeField, PropertyOrder(-1), HideLabel, InlineProperty]
    private Data _data;

    private CapsuleCollider _Collider;

    [System.Serializable]
    public class Data
    {
        public float Durability = 10;
        [HideInInspector] public float MaxDurability;
    }

    public Data OnUpload() => _data;
    public void OnLoad(object state)
    {
        _data = state as Data;

        bool destroyed = _data.Durability <= 0;

        if (_Tree != null)
            _Tree.SetActive(!destroyed);
        if (_Stump != null)
            _Stump.SetActive(destroyed);
    }
    void IStreamable.OnBeforeSaving()
    {

    }

    [SerializeField]
    ItemLabels IUsable.Filter() => _filter;

    private void Awake()
    {
        _data.MaxDurability = _data.Durability;
        _Collider = GetComponent<CapsuleCollider>();
    }

    void IUsable.OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor) || _data.Durability <= 0)
            return;

        float previousDurability = _data.Durability;
        context.Damage(ref _data.Durability, new MinMaxFloat(1, 2), _star);

        if (_shakeTransform != null)
            context.Shake(_shakeTransform, _shakeDuration, strength: _shakeStrength, angleRotation: _shakeAngleRotation);

        if (_particleSystem != null)
            _particleSystem.Emit(_woodAmount);

        if (_particleSystemLeaf != null)
            _particleSystemLeaf.Emit(_leafAmount);

        if (_data.Durability < previousDurability)
            SoundPlayer.Instance.Play(_hitSound);
        else
            SoundPlayer.Instance.Play("hit_poor");

        if (_data.Durability <= 0)
        {
            SoundPlayer.Instance.Play(_breakSound);

            context.Drop(transform.TransformPoint(_Collider.center), _drops);

            if (_Tree != null)
                _Tree.SetActive(false);
            if (_Stump != null)
                _Stump.SetActive(true);

            if (TryGetComponent(out StreamableBehaviour streamable))
                streamable.SetToRespawn();
        }
    }
}
