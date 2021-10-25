using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;
using Sirenix.Serialization;

/// <summary>
/// Manages all of the flora in this region
/// </summary>
[CreateAssetMenu(fileName = "FloraMaster", menuName = "Game Logic/Flora Master")]
[System.Serializable]
public class FloraMaster : SerializedScriptableSingleton<FloraMaster>, IStreamer
{
    [SerializeField]
    private GameObject _FloraPrefab;
    [SerializeField]
    private System.Guid _Guid;
    [OdinSerialize]
    private Dictionary<string, FloraInfo> _FloraDictionary = new Dictionary<string, FloraInfo>();

    [System.NonSerialized]
    private List<Flora> _Florae = new List<Flora>();

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();

        GameManager.AddStreamer(this);
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += Instance.OnSceneLoaded;
        GameManager.AddStreamer(FloraMaster.Instance);
    }
#endif

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (Flora flora in _Florae)
        {
            if (scene.buildIndex == flora.SaveData.SceneIndex)
            {
                Cell cell = Grid.Instance.GetCellLocal(flora.SaveData.CellPosition);

                Grid.Instance.UpdateCellsUVs(flora.SaveData.Watered ? CellType.Soil : CellType.Dirt, cell);
                cell.Occupied = false;

                Create(flora, cell);
            }
        }
    }

    public bool Add(string floraName)
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell == null || cell.Occupied)
            return false;

        if (cell.Data.Type != CellType.Dirt && cell.Data.Type != CellType.Soil)
            return false;

        string key = floraName.ToLower();

        if (!_FloraDictionary.ContainsKey(key))
            return false;

        Flora flora = new Flora(_FloraDictionary[key], cell.Local);

        if (!Create(flora, cell))
            return false;

        _Florae.Add(flora);

        return true;
    }
    public bool Add(FloraInfo floraInfo)
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell == null || cell.Occupied)
            return false;

        if (cell.Data.Type != CellType.Dirt && cell.Data.Type != CellType.Soil)
            return false;

        Flora flora = new Flora(floraInfo, cell.Local);

        if (!Create(flora, cell))
            return false;

        _Florae.Add(flora);

        return true;
    }

    public bool Create(Flora flora, Cell cell)
    {
        GameObject obj = Instantiate(_FloraPrefab);

        if (!Grid.Instance.OccupyCell(cell, obj))
        {
            Destroy(obj);
            return false;
        }

        FloraObject floraObject = obj.GetComponent<FloraObject>();
        floraObject.Initialize(flora);

        return true;
    }

    public bool Remove(Flora flora)
    {
        if (!_Florae.Contains(flora))
            return false;

        _Florae.Remove(flora);

        return true;
    }

    public void Notify()
    {
        _Florae.ForEach(f => f.Grow());
    }

    public void UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        _Florae.ForEach(x =>
            streamables.Add(
                x.Cell.Local.x.ToString() + "," +
                x.Cell.Local.y.ToString() + " " +
                x.SaveData.SceneIndex.ToString(), ((IStreamable<object>)x).OnUpload()));

        GameManager.Stream.Load(_Guid, streamables);
    }
    public void Load()
    {
        _Florae = new List<Flora>();

        Dictionary<string, object> streamables = (Dictionary<string, object>)GameManager.Stream.Get(_Guid);

        foreach(var item in streamables)
        {
            if (!streamables.TryGetValue(item.Key, out object value))
                continue;

            Flora.Data data = value as Flora.Data;

            Flora flora = new Flora(_FloraDictionary[data.Name], data.CellPosition);
            flora.OnLoad(data);

            _Florae.Add(flora);
        }
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();

        _Florae.ForEach(x => ((IStreamable<object>)x).OnBeforeSaving());
    }
}
