using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls the flora logic
/// </summary>
public class Flora : IStreamable<Flora.Data>
{
    private FloraInfo _Info; // data used to create flora
    private Data _Data;

    public class Data
    {
        public string Name;
        public Vector2Int CellPosition;
        public IHarvest HarvestMethod;
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
    public void OnBeforeSaving() 
    { 

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

    public FloraInfo Info => _Info;
    public Data FloraData => _Data;
    public Cell Cell => Grid.Instance.GetCellLocal(_Data.CellPosition);

    public bool Completed => (_Data.Stage >= _Info.GrowTime);

    public Flora(FloraInfo floraInfo, Vector2Int cellPos)
    {
        _Info = floraInfo;
        _Data = new Data();

        _Data.Name = _Info.Name;
        _Data.CellPosition = cellPos;
        _Data.SceneIndex = SceneManager.GetActiveScene().buildIndex;

        _StagesMeshFilters = System.Array.ConvertAll(_Info.Stages, mf => mf.GetComponent<MeshFilter>()); // extract mesh and materials from objects
        _StagesMeshRenderers = System.Array.ConvertAll(_Info.Stages, mr => mr.GetComponent<MeshRenderer>());

        _StageUpdate = (_Info.GrowTime + 1f) / _Info.Stages.Length;

        _Data.HarvestMethod = (IHarvest)System.Activator.CreateInstance(_Info.HarvestMethod.GetType()); // create a new instance of the harvest method
        _Data.HarvestMethod.Initialize(_Info, _Info.HarvestMethod); 
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

        _Data.Watered = false;
    }

    public bool Water()
    {
        if (_Data.Watered)
            return false;

        OnWatered.Invoke();

        return _Data.Watered = true;
    }

    public bool Harvest(UsedContext context)
    {
        return _Data.HarvestMethod.Harvest(context, this);
    }
}
