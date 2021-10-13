using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls logic related to the object itself
/// </summary>
public class FloraObject : MonoBehaviour, IUsable, IDestructable
{
    private Flora _Flora;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;
    private BoxCollider _Collider;

    public Flora Flora => _Flora;

    public int Priority => 1;
    public float Strength { get; set; } = 1;
    public float Durability { get; set; } = 1;

    private void OnDestroy()
    {
        _Flora.OnGrow -= UpdateMesh;
    }

    public void Initialize(Flora flora)
    {
        _Flora = flora;

        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        _Collider = GetComponent<BoxCollider>();

        _MeshFilter.mesh = _Flora.CurrentMeshFilter.sharedMesh; // set new mesh and materials on this object
        _MeshRenderer.materials = _Flora.CurrentMeshRenderer.sharedMaterials;

        _Flora.OnGrow += UpdateMesh;

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        _MeshFilter.sharedMesh = _Flora.CurrentMeshFilter.sharedMesh;
        _MeshRenderer.sharedMaterials = _Flora.CurrentMeshRenderer.sharedMaterials;

        transform.position = _Flora.Tile.Middle;
        transform.position += Vector3.up * (_MeshFilter.sharedMesh.bounds.size.y / 2.0f);

        _Collider.center = new Vector3(0, 
            _MeshFilter.sharedMesh.bounds.center.y + 
            (_Collider.bounds.size.y - _MeshFilter.sharedMesh.bounds.size.y) / 2.0f, 0);
    }

    public void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Flora.HarvestMethod.Harvest(Flora, 0);
        }
    }

    public IList<string> Filter(ItemLabels labels) => Flora.HarvestMethod.Filter(labels);
    public void OnUse(UsedContext context)
    {
        if (!context.performed)
            return;

        Flora.OnHarvest.Invoke();

        Flora.HarvestMethod.Harvest(_Flora, context.playerIndex);
    }
}
