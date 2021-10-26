using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class DebugUseArea : MonoBehaviour, IUsable
{
    [SerializeField, Required] private Material _firstMaterial;
    [SerializeField, Required] private Material _secondMaterial;

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
}
