using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls logic related to the object itself
/// </summary>
public class FloraObject : MonoBehaviour, IUsable
{
    [SerializeField]
    private Material _DryMaterial;
    [SerializeField]
    private Material _WateredMaterial;

    private Flora _Flora;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;
    private BoxCollider _Collider;

    public Flora Flora => _Flora;

    private void OnDestroy()
    {
        if (_Flora != null)
        {
            _Flora.OnGrow -= UpdateMesh;
            _Flora.OnWatered -= Watered;
        }
    }

    public void Initialize(Flora flora)
    {
        _Flora = flora;

        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        _Collider = GetComponent<BoxCollider>();

        _Flora.OnGrow += UpdateMesh;
        _Flora.OnWatered += Watered;

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        if (_Flora.CurrentMeshFilter != null || _Flora.CurrentMeshRenderer != null)
        {
            _MeshFilter.mesh = _Flora.CurrentMeshFilter.sharedMesh;
            _MeshRenderer.materials = _Flora.CurrentMeshRenderer.sharedMaterials;

            Material[] materials = new Material[_MeshRenderer.materials.Length];
            for (int i = 0; i < materials.Length; ++i)
            {
                materials[i] = (_Flora.CurrentMeshRenderer.sharedMaterials[i] == _WateredMaterial) ? _DryMaterial : _MeshRenderer.materials[i];
            }
            _MeshRenderer.materials = materials;
        }

        transform.position = _Flora.Cell.Middle;

        _Collider.center = new Vector3(0, 
            _MeshFilter.mesh.bounds.center.y + 
            (_Collider.bounds.size.y - _MeshFilter.mesh.bounds.size.y) / 2.0f, 0);
    }

    private void Watered()
    {
        if (_Flora.CurrentMeshRenderer != null)
        {
            Material[] materials = new Material[_MeshRenderer.materials.Length];
            for (int i = 0; i < materials.Length; ++i)
            {
                materials[i] = (_Flora.CurrentMeshRenderer.sharedMaterials[i] == _DryMaterial) ? _WateredMaterial : _MeshRenderer.materials[i];
            }
            _MeshRenderer.materials = materials;
        }
    }

    public void OnUse(UsedContext context)
    {
        if (!(context.used is IDestructor))
            return;

        foreach (string label in context.labels)
        {
            if (Enum.TryParse(label, true, out ItemLabels itemLabel) && itemLabel == ItemLabels.Pickaxe)
            {
                DestroyFlora();
                return;
            }
        }

        if (Flora.Harvest(context))
            DestroyFlora();

        void DestroyFlora()
        {
            Destroy(_Flora.Cell.Free());
            FloraMaster.Instance.Remove(_Flora);

            SoundPlayer.Instance.Play(_Flora.Info.HarvestSound, 0, 0, 0, 0.015f);
        };
    }

    ItemLabels IUsable.Filter() => _Flora.Info.ItemLabels;
}
