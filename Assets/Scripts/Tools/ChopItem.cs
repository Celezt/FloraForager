using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;

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

        Collider[] colliders = Physics.OverlapBox(context.transform.position + context.transform.rotation * _centerOffset, _halfExtents, context.transform.rotation, LayerMask.NameToLayer("default"));
        for (int i = 0; i < colliders.Length; i++)
            if (colliders[i].TryGetComponent(out IUsable usable))
                yield return usable;
    }
}
