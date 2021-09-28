using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloraObject : MonoBehaviour, IInteractable
{
    private FloraData _Flora;

    private MeshFilter[] _StagesMeshFilters;
    private MeshRenderer[] _StagesMeshRenderers;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;
    private BoxCollider _Collider;

    private Tile _Tile;             // associated tile

    private float _StageUpdate = 0; // at what stages to update the mesh
    private int _Stage = 0;         // current stage/day
    private int _Mesh = 0;          // current mesh being used

    public int Priority => 1;

    public void Initialize(FloraData flora, Tile tile)
    {
        _Flora = flora;
        _Tile = tile;

        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        _Collider = GetComponent<BoxCollider>();

        _StagesMeshFilters = System.Array.ConvertAll(_Flora.Stages, mf => mf.GetComponent<MeshFilter>()); // extract mesh and materials from objects
        _StagesMeshRenderers = System.Array.ConvertAll(_Flora.Stages, mr => mr.GetComponent<MeshRenderer>());

        if (_StagesMeshFilters.Length == 0 || _StagesMeshRenderers.Length == 0)
            Debug.LogError(name + " has no stage variants assigned to it");

        _MeshFilter.mesh = _StagesMeshFilters[_Mesh].sharedMesh; // set new mesh and materials on this object
        _MeshRenderer.materials = _StagesMeshRenderers[_Mesh].sharedMaterials;

        _StageUpdate = (_Flora.GrowTime + 1) / (float)_Flora.Stages.Length;

        transform.position += Vector3.up * _MeshFilter.mesh.bounds.size.y / 2.0f;
        _Collider.center = new Vector3(0,
            _MeshFilter.sharedMesh.bounds.center.y +
            (_Collider.bounds.size.y - _MeshFilter.sharedMesh.bounds.size.y) / 2.0f, 0);
    }

    public void Grow()
    {
        if (_Stage >= _Flora.GrowTime)
            return;

        if (++_Stage >= (_StageUpdate + _Mesh))
        {
            ++_Mesh;

            _MeshFilter.sharedMesh = _StagesMeshFilters[_Mesh].sharedMesh;
            _MeshRenderer.sharedMaterials = _StagesMeshRenderers[_Mesh].sharedMaterials;

            transform.position = _Tile.Middle;
            transform.position += Vector3.up * (_MeshFilter.sharedMesh.bounds.size.y / 2.0f);

            _Collider.center = new Vector3(0, 
                _MeshFilter.sharedMesh.bounds.center.y + 
                (_Collider.bounds.size.y - _MeshFilter.sharedMesh.bounds.size.y) / 2.0f, 0);
        }
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        if (_Stage >= _Flora.GrowTime) // final stage reached
        {
            for (int i = 0; i < _Flora.Rewards.Length; ++i)
            {
                string itemID = _Flora.Rewards[i].ID;
                int amount = _Flora.Rewards[i].Amount;

                // access inventory and add rewards
            }
            Debug.Log("reward"); // access inventory and add rewards
        }
        else
        {
            Debug.Log("no reward");
        }

        GridInteraction.RemoveObject(_Tile);
        FloraMaster.Instance.RemoveFlora(this);
    }
}
