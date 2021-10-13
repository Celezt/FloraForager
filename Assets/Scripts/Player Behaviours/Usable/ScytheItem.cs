using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

public class ScytheItem : IUse, IItem, IDestructor
{
    [OdinSerialize]
    public uint ItemStack { get; set; } = 1;
    [OdinSerialize]
    public float Cooldown { get; set; } = 0.5f;
    [OdinSerialize]
    public float Strength { get; set; } = DurabilityStrengths.BRITTLE_STONE;
    [OdinSerialize]
    public float Damage { get; set; } = 2.0f;

    [SerializeField, AssetsOnly]
    private GameObject _modelPrefab { get; set; }

    [SerializeField]
    private float _radius = 3f;
    [SerializeField]
    private float _arc = 0.4f;

    private Transform _scytheTransform;
    private Animator _animator;

    void IItem.OnEquip(ItemContext context)
    {
        _scytheTransform = GameObject.Instantiate(_modelPrefab, context.playerTransform.position, Quaternion.identity).transform;
        _scytheTransform.parent = context.playerTransform;

        _animator = _scytheTransform.GetComponent<Animator>();
    }

    void IItem.OnUnequip(ItemContext context)
    {
        GameObject.Destroy(_scytheTransform);
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        _animator.SetTrigger("hit");

        Collider[] colliders = PhysicsC.OverlapArc(context.playerTransform.position, context.playerTransform.forward, Vector3.up, _radius, _arc, LayerMask.NameToLayer("default"));
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].TryGetComponent(out IUsable usable))
                yield return usable;
    }
}
