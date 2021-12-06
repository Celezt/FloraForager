using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RockBehaviour : MonoBehaviour, IStreamable<RockBehaviour.Data>, IUsable
{
    [Header("Properties")]
    [SerializeField] private string _hitSound = "hit_rock";
    [SerializeField] private string _breakSound = "break_wood";
    [SerializeField] private ItemLabels _filter = ItemLabels.Pickaxe;
    [SerializeField] private Stars _star = Stars.One;
    [SerializeField] private List<DropType> _drops = new List<DropType>();
    [Header("Shake Settings")]
    [SerializeField] private Transform _shakeTransform;
    [SerializeField] private float _shakeDuration = 2.0f;
    [SerializeField] private float _shakeStrength = 0.05f;
    [SerializeField] private float _shakeAngleRotation = 0.0f;
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem _particleSystem;

    [SerializeField, PropertyOrder(-1), HideLabel, InlineProperty]
    private Data _data;

    private BoxCollider _Collider;

    [System.Serializable]
    public class Data
    {
        public float Durability = 10;
        [HideInInspector] public float MaxDurability;
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

    private void Awake()
    {
        _data.MaxDurability = _data.Durability;
    }
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
            context.Shake(_shakeTransform, _shakeDuration, strength: _shakeStrength, angleRotation: _shakeAngleRotation);

        if (_particleSystem != null)
            _particleSystem.Emit(100);

        if (_data.Durability < previousDurability)
            SoundPlayer.Instance.Play(_hitSound);
        else
            SoundPlayer.Instance.Play("hit_poor");

        if (_data.Durability <= 0)
        {
            SoundPlayer.Instance.Play(_breakSound);

            context.Drop(transform.TransformPoint(_Collider.center), _drops);
            gameObject.SetActive(false);
        }
    }
}
