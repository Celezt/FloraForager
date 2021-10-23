using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class DebugUseArea : MonoBehaviour, IUsable, IResourceObject, IDestructableObject
{
    [SerializeField, Required] private Material _firstMaterial;
    [SerializeField, Required] private Material _secondMaterial;

    [ItemTypeWithInterface(typeof(IResource))]
    public ItemType scriptable;

    private MeshRenderer _meshRenderer;

    private bool _isToggled;

    ItemLabels IUsable.Filter() => ItemLabels.Scythe | ItemLabels.Axe;

    void IUsable.OnUse(UsedContext context)
    {
        if (context.used is IDestructor)
        {
            _isToggled = !_isToggled;

            _meshRenderer.material = _isToggled ? _firstMaterial : _secondMaterial;
        }
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    void IDestructableObject.OnDamage(IDestructor destructor, IDestructable destructable, UsedContext context)
    {
        Debug.Log(destructor.Damage);
    }

    void IDestructableObject.OnDestruction(UsedContext context)
    {
        Debug.Log("destroyed");
    }
}
