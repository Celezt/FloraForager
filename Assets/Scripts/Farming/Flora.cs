using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flora : MonoBehaviour, IInteractable
{
    [Space(3), Header("Properties")]
    [SerializeField] private string _Name;
    [SerializeField, TextArea(3, 10)] private string _Description;
    //[SerializeField] private List<Item> _Rewards;

    [Space(3), Header("Growth")]
    [SerializeField, Min(0)] private int _GrowTime = 0; // growth time in days
    [SerializeField] private GameObject[] _Stages; // Number of visual growth stages this flora has [0 = start, x = final]

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

    public string Name => _Name;
    public string Description => _Description;

    private void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        _Collider = GetComponent<BoxCollider>();

        _StagesMeshFilters = System.Array.ConvertAll(_Stages, mf => mf.GetComponent<MeshFilter>()); // extract mesh and materials from objects
        _StagesMeshRenderers = System.Array.ConvertAll(_Stages, mr => mr.GetComponent<MeshRenderer>());

        if (_StagesMeshFilters.Length == 0 || _StagesMeshRenderers.Length == 0)
            Debug.LogError(name + " has no stage variants assigned to it");

        _MeshFilter.sharedMesh = _StagesMeshFilters[_Mesh].sharedMesh; // set new mesh and materials on this object
        _MeshRenderer.sharedMaterials = _StagesMeshRenderers[_Mesh].sharedMaterials;
    }

    private void Start()
    {
        _StageUpdate = (_GrowTime + 1) / (float)_Stages.Length;

        _Collider.center = new Vector3(0,
            _MeshFilter.sharedMesh.bounds.center.y +
            (_Collider.bounds.size.y - _MeshFilter.sharedMesh.bounds.size.y) / 2.0f, 0);
    }

    public void Initialize(Tile tile)
    {
        _Tile = tile;
    }

    public void Grow()
    {
        if (_Stage >= _GrowTime)
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

        if (_Stage >= _GrowTime) // final stage reached
        {
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
