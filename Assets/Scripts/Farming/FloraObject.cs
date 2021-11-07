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
    private Flora _Flora;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;
    private BoxCollider _Collider;

    public Flora Flora => _Flora;

    private void OnDestroy()
    {
        if (_Flora != null)
            _Flora.OnGrow -= UpdateMesh;
    }

    public void Initialize(Flora flora)
    {
        _Flora = flora;

        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        _Collider = GetComponent<BoxCollider>();

        if (_Flora.CurrentMeshFilter != null && _Flora.CurrentMeshRenderer != null)
        {
            _MeshFilter.mesh = _Flora.CurrentMeshFilter.sharedMesh; // set new mesh and materials on this object
            _MeshRenderer.materials = _Flora.CurrentMeshRenderer.sharedMaterials;
        }

        _Flora.OnGrow += UpdateMesh;

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        if (_Flora.CurrentMeshFilter != null && _Flora.CurrentMeshRenderer != null)
        {
            _MeshFilter.mesh = _Flora.CurrentMeshFilter.sharedMesh;
            _MeshRenderer.materials = _Flora.CurrentMeshRenderer.sharedMaterials;
        }

        transform.position = _Flora.Cell.Middle;
        transform.position += Vector3.up * _MeshFilter.mesh.bounds.extents.y;

        _Collider.center = new Vector3(0, 
            _MeshFilter.mesh.bounds.center.y + 
            (_Collider.bounds.size.y - _MeshFilter.mesh.bounds.size.y) / 2.0f, 0);
    }

    public void OnUse(UsedContext context)
    {
        if (Flora.Harvest(context))
        {
            Destroy(_Flora.Cell.Free());
            FloraMaster.Instance.Remove(_Flora);
        }
    }

    ItemLabels IUsable.Filter() => _Flora.Info.ItemLabels;
}
