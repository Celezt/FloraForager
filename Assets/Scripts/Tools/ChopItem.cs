using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.InputSystem;

public class ChopItem : IItem, IUse, IDestructor
{
    [OdinSerialize, PropertyOrder(float.MinValue)]
    int IItem.ItemStack { get; set; } = 1;
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    float IUse.Cooldown { get; set; } = 1.0f;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    float IDestructor.Strength { get; set; } = DurabilityStrengths.BRITTLE_STONE;
    [OdinSerialize, PropertyOrder(float.MinValue + 3)]
    float IDestructor.Damage { get; set; } = 2.0f;

    [SerializeField]
    private float _stunDuration = 0.8f;
    [SerializeField]
    private Vector3 _halfExtents = new Vector3(0.5f, 1.0f, 0.5f);
    [SerializeField]
    private Vector3 _centerOffset = new Vector3(0, 0, 1f);

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
#if UNITY_EDITOR
        context.useBehaviour.OnDrawGizmosAction = () =>
        {
            Gizmos.matrix = context.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(_centerOffset, _halfExtents * 2);
        };
#endif
    }

    void IItem.OnUnequip(ItemContext context)
    {

    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        context.transform.GetComponent<PlayerMovement>().SpeedMultipliers.Add(_stunDuration, 0);
        
        Collider[] colliders = Physics.OverlapBox(context.transform.position + context.transform.rotation * _centerOffset, _halfExtents, context.transform.rotation, LayerMask.NameToLayer("default"));
        List<Collider> usableColliders = new List<Collider>(colliders.Length);

        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].TryGetComponent(out IUsable _))   // Only call one usable.
                usableColliders.Add(colliders[i]);
        
        Collider collider = new KdTree<Collider>(usableColliders).FindClosest(context.transform.position);

        if (collider == null)
            yield break;

        yield return collider.GetComponent<IUsable>();
    }
}
