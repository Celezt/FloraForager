using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Celezt.Mathematics;
using MyBox;

public class ScytheItem : IUse, IItem, IDestructor
{
    [OdinSerialize, PropertyOrder(float.MinValue)]
    public uint ItemStack { get; set; } = 1;
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    public float Cooldown { get; set; } = 0.5f;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    public float Strength { get; set; } = DurabilityStrengths.BRITTLE_STONE;
    [OdinSerialize, PropertyOrder(float.MinValue + 3)]
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
        _scytheTransform = GameObject.Instantiate(_modelPrefab, context.transform.position, Quaternion.identity).transform;
        _scytheTransform.parent = context.transform;

        _animator = _scytheTransform.GetComponent<Animator>();

        context.useBehaviour.OnDrawGizmosAction = () =>
        {
            Gizmos.matrix = context.transform.localToWorldMatrix;
            GizmosC.DrawWireArc(Vector3.zero, Vector3.forward, _radius, cmath.Map(_arc, new MinMaxFloat(1, -1), new MinMaxFloat(0, 360)));
        };
    }

    void IItem.OnUnequip(ItemContext context)
    {
        GameObject.Destroy(_scytheTransform.gameObject);
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        _animator.SetTrigger("hit");

        Collider[] colliders = PhysicsC.OverlapArc(context.transform.position, context.transform.forward, Vector3.up, _radius, _arc, LayerMask.NameToLayer("default"));
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].TryGetComponent(out IUsable usable))
                yield return usable;
    }
}
