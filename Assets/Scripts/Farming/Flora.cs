using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the flora logic
/// </summary>
public class Flora : IStreamable<Flora.Data>
{
    private FloraInfo _FloraInfo; // data used to create flora
    private Data _Data;

    public class Data
    {
        public string Name;
        public Vector2Int CellPosition;
        public IHarvest Harvest;
        public int Stage;
        public int Mesh;
        public bool Watered;
        public int SceneIndex;
    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }

    public System.Action OnGrow = delegate { };
    public System.Action OnWatered = delegate { };
    public System.Action OnCompleted = delegate { };
    public System.Action OnHarvest = delegate { };

    private MeshFilter[] _StagesMeshFilters;
    private MeshRenderer[] _StagesMeshRenderers;

    private float _StageUpdate = 0; // at what stages to update the mesh

    public MeshFilter CurrentMeshFilter => _StagesMeshFilters[_Data.Mesh];
    public MeshRenderer CurrentMeshRenderer => _StagesMeshRenderers[_Data.Mesh];

    public FloraInfo FloraInfo => _FloraInfo;
    public Data SaveData => _Data;
    public Cell Cell => Grid.Instance.GetCellLocal(_Data.CellPosition);

    public bool Completed => (_Data.Stage >= _FloraInfo.GrowTime);
    public bool Watered
    {
        get => _Data.Watered;
        set
        {
            OnWatered.Invoke();
            Grid.Instance.UpdateCellsUVs((_Data.Watered = value) ? CellType.Soil : CellType.Dirt, Cell);
        }
    }

    public Flora(FloraInfo floraInfo, Vector2Int cellPos)
    {
        _FloraInfo = floraInfo;
        _Data = new Data();

        _Data.Name = _FloraInfo.Name;
        _Data.CellPosition = cellPos;
        _Data.SceneIndex = SceneManager.GetActiveScene().buildIndex;

        _StagesMeshFilters = System.Array.ConvertAll(_FloraInfo.Stages, mf => mf.GetComponent<MeshFilter>()); // extract mesh and materials from objects
        _StagesMeshRenderers = System.Array.ConvertAll(_FloraInfo.Stages, mr => mr.GetComponent<MeshRenderer>());

        _StageUpdate = (_FloraInfo.GrowTime + 1f) / _FloraInfo.Stages.Length;

        _Data.Harvest = (IHarvest)System.Activator.CreateInstance(_FloraInfo.HarvestMethod.GetType()); // create a new instance of the harvest method
        _Data.Harvest.Initialize(_FloraInfo, _FloraInfo.HarvestMethod); // fill it with new data
    }

    public void Grow()
    {
        if (Completed || !_Data.Watered)
            return;

        if (++_Data.Stage >= (_StageUpdate + _Data.Mesh))
        {
            ++_Data.Mesh; // update to next mesh
        }

        OnGrow.Invoke();

        if (Completed)
            OnCompleted.Invoke();

        Watered = false;
    }
}
