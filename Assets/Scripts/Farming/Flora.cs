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
    [SerializeField] private Mesh[] _Stages;            // number of visual growth stages this flora has [0 = start, x = final]

    private MeshFilter _MeshFilter;
    private BoxCollider _Collider;

    private FloraMaster _FloraMaster; // master
    private Tile _Tile;               // associated tile

    private float _StageUpdate = 0; // at what stages to update the mesh
    private int _Stage = 0;         // current stage/day
    private int _Mesh = 0;          // current mesh being used

    public int Priority => 1;

    public string Name => _Name;
    public string Description => _Description;

    private void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _Collider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        _MeshFilter.mesh = _Stages[_Mesh];
        _StageUpdate = (_GrowTime + 1) / (float)_Stages.Length;

        _Collider.center = new Vector3(0, 
            _MeshFilter.mesh.bounds.center.y + 
            (_Collider.bounds.size.y - _MeshFilter.mesh.bounds.size.y) / 2.0f, 0);
    }

    private void Update()
    {
        
    }

    public void Create(FloraMaster floraMaster, Tile tile)
    {
        _FloraMaster = floraMaster;
        _Tile = tile;
    }

    public void Grow()
    {
        if (_Stage >= _GrowTime)
            return;

        if (++_Stage >= (_StageUpdate + _Mesh))
        {
            _MeshFilter.mesh = _Stages[++_Mesh];

            transform.position = _Tile.Middle;
            transform.position += Vector3.up * (_MeshFilter.mesh.bounds.size.y / 2.0f);

            _Collider.center = new Vector3(0, 
                _MeshFilter.mesh.bounds.center.y + 
                (_Collider.bounds.size.y - _MeshFilter.mesh.bounds.size.y) / 2.0f, 0);
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
        _FloraMaster.RemoveFlora(this);
    }
}
