using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the flora logic
/// </summary>
public class Flora : IStreamable<Flora.FloraData>
{
    private global::FloraData _Data; // data used to create flora
    private Cell _Cell;      // associated cell

    private FloraData _data;

    public class FloraData
    {
        public int Stage;
        public bool Watered;
    }

    FloraData IStreamable<FloraData>.OnUpload() => _data = new FloraData();

    void IStreamable.OnLoad(object state)
    {
        _data = state as FloraData;
    }

    void IStreamable.OnBeforeSaving() { }

    public System.Action OnGrow = delegate { };
    public System.Action OnWatered = delegate { };
    public System.Action OnCompleted = delegate { };
    public System.Action OnHarvest = delegate { };

    private MeshFilter[] _StagesMeshFilters;
    private MeshRenderer[] _StagesMeshRenderers;

    private float _StageUpdate = 0; // at what stages to update the mesh
    //private int _Stage = 0;         // current advanced stage
    private int _Mesh = 0;          // current mesh being used

    //private bool _Watered;          // if the flora has been watered for this day

    public MeshFilter CurrentMeshFilter => _StagesMeshFilters[_Mesh];
    public MeshRenderer CurrentMeshRenderer => _StagesMeshRenderers[_Mesh];

    public global::FloraData Data => _Data;
    public Cell Cell => _Cell;

    public IHarvest HarvestMethod { get; private set; }

    public bool Completed => (_data.Stage >= _Data.GrowTime);
    public bool Watered
    {
        get => _data.Watered;
        set
        {
            OnWatered.Invoke();
            _Cell.Grid.UpdateCellsUVs((_data.Watered = value) ? CellType.Soil : CellType.Dirt, _Cell);
        }
    }

    public Flora(global::FloraData data, Cell cell)
    {
        _Data = data;
        _Cell = cell;

        _StagesMeshFilters = System.Array.ConvertAll(_Data.Stages, mf => mf.GetComponent<MeshFilter>()); // extract mesh and materials from objects
        _StagesMeshRenderers = System.Array.ConvertAll(_Data.Stages, mr => mr.GetComponent<MeshRenderer>());

        _StageUpdate = (_Data.GrowTime + 1f) / _Data.Stages.Length;

        HarvestMethod = (IHarvest)System.Activator.CreateInstance(_Data.HarvestMethod.GetType()); // create a new instance of the harvest method
        HarvestMethod.Initialize(data, _Data.HarvestMethod); // fill it with new data
    }

    public void Grow()
    {
        if (Completed || !_data.Watered)
            return;

        if (++_data.Stage >= (_StageUpdate + _Mesh))
        {
            ++_Mesh; // update to next mesh
        }

        OnGrow.Invoke();

        if (Completed)
            OnCompleted.Invoke();

        Watered = false;
    }
}
