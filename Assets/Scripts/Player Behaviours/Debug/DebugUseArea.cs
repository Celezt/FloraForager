using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DebugUseArea : MonoBehaviour, IUsable, IDestructable
{
    public float Strength { get; set; } = DurabilityStrengths.UNARMED;
    public float Durability { get; set; } = 10;

    [SerializeField, Required] private Material _firstMaterial;
    [SerializeField, Required] private Material _secondMaterial;

    private MeshRenderer _meshRenderer;

    private bool _isToggled;

    public IList<string> Filter(ItemLabels labels) => new List<string> { labels.SCYTHE, labels.AXE };

    public void OnUse(UsedContext context)
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
