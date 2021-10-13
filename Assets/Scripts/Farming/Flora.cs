using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the flora logic
/// </summary>
public class Flora
{
    private FloraData _Data; // data used to create flora
    private Tile _Tile;      // associated tile

    public System.Action OnGrow = delegate { };
    public System.Action OnWatered = delegate { };
    public System.Action OnCompleted = delegate { };
    public System.Action OnHarvest = delegate { };

    private MeshFilter[] _StagesMeshFilters;
    private MeshRenderer[] _StagesMeshRenderers;

    private float _StageUpdate = 0; // at what stages to update the mesh
    private int _Stage = 0;         // current advanced stage
    private int _Mesh = 0;          // current mesh being used

    private bool _Watered;          // if the flora has been watered for this day

    public MeshFilter CurrentMeshFilter => _StagesMeshFilters[_Mesh];
    public MeshRenderer CurrentMeshRenderer => _StagesMeshRenderers[_Mesh];

    public FloraData Data => _Data;
    public Tile Tile => _Tile;

    public IHarvest HarvestMethod { get; private set; }

    public bool Completed => (_Stage >= _Data.GrowTime);
    public bool Watered
    {
        get => _Watered;
        set
        {
            OnWatered.Invoke();
            _Tile.TileMap.Grid.UpdateTile((_Watered = value) ? TileType.Soil : TileType.Dirt, _Tile);
        }
    }

    public Flora(FloraData data, Tile tile)
    {
        _Data = data;
        _Tile = tile;

        _StagesMeshFilters = System.Array.ConvertAll(_Data.Stages, mf => mf.GetComponent<MeshFilter>()); // extract mesh and materials from objects
        _StagesMeshRenderers = System.Array.ConvertAll(_Data.Stages, mr => mr.GetComponent<MeshRenderer>());

        _StageUpdate = (_Data.GrowTime + 1f) / _Data.Stages.Length;

        HarvestMethod = (IHarvest)System.Activator.CreateInstance(_Data.HarvestMethod.GetType()); // create a new instance of the harvest method
        HarvestMethod.Initialize(data);
    }

    public void Grow()
    {
        if (Completed || !_Watered)
            return;

        if (++_Stage >= (_StageUpdate + _Mesh))
        {
            ++_Mesh; // update to next mesh
        }

        OnGrow.Invoke();

        if (Completed)
            OnCompleted.Invoke();

        Watered = false;
    }
}
