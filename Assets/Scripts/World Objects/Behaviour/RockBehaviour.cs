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
    [SerializeField] private ParticleSystem _particleSystemPuff;
    [SerializeField] private int _rockAmount = 0;
    [SerializeField] private int _puffAmount = 0;
    [Header("Objects")]
    [SerializeField] private GameObject _Rock;
    [Header("Colliders")]
    [SerializeField] private BoxCollider _RockCollider;

    [SerializeField, PropertyOrder(-1), HideLabel, InlineProperty]
    private Data _data;

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

        if (_Rock != null)
            _Rock.SetActive(!destroyed);
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

    void IUsable.OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor) || _data.Durability <= 0)
            return;

        if (context.Damage(ref _data.Durability, new MinMaxFloat(1, 2), _star))
        {
            if (_shakeTransform != null)
                context.Shake(_shakeTransform, _shakeDuration, strength: _shakeStrength, angleRotation: _shakeAngleRotation);

            if (_particleSystem != null)
                _particleSystem.Emit(_rockAmount);

            SoundPlayer.Instance.Play(_hitSound);
        }
        else
        {
            MessageLog.Instance.Send("Inadequate Tool", Color.red, 14f, 2f);
            SoundPlayer.Instance.Play("hit_poor");
        }

        if (_data.Durability <= 0)
        {
            SoundPlayer.Instance.Play(_breakSound);

            if (_particleSystemPuff != null)
                _particleSystemPuff.Emit(_puffAmount);

            context.Drop(transform.TransformPoint(_RockCollider.center), _drops);

            if (_Rock != null)
                _Rock.SetActive(false);

            if (TryGetComponent(out StreamableBehaviour streamable))
                streamable.SetToRespawn();
        }
    }
}
