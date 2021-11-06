using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Celezt.Mathematics;
using MyBox;

public class ScytheItem : IUse, IDestructor, IStar, IValue
{
    [OdinSerialize, PropertyOrder(float.MinValue)]
    int IItem.ItemStack { get; set; } = 1;
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    int IValue.BaseValue { get; set; }
    [OdinSerialize, PropertyOrder(float.MinValue + 3)]
    float IUse.Cooldown { get; set; } = 0.5f;
    [OdinSerialize, PropertyOrder(float.MinValue + 4)]
    float IDestructor.Damage { get; set; } = 2.0f;

    [Title("Tool Behaviour")]
    [SerializeField]
    private float _stunDuration = 0.5f;
    [SerializeField]
    private float _radius = 3f;
    [SerializeField]
    private float _arc = 0.4f;
    [SerializeField, AssetsOnly]
    private GameObject _modelPrefab { get; set; }

    private Transform _scytheTransform;
    private Animator _animator;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
        _scytheTransform = GameObject.Instantiate(_modelPrefab, context.transform.position, Quaternion.identity).transform;
        _scytheTransform.parent = context.transform;

        _animator = _scytheTransform.GetComponent<Animator>();

        context.behaviour.OnDrawGizmosAction = () =>
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

        context.transform.GetComponent<PlayerMovement>().ActivaInput.Add(_stunDuration);

        Collider[] colliders = PhysicsC.OverlapArc(context.transform.position, context.transform.forward, Vector3.up, _radius, _arc, LayerMask.NameToLayer("default"));

        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].TryGetComponent(out IUsable usable))
                yield return usable;
    }
}
