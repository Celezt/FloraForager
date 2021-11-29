using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;
using Sirenix.Serialization;
using IngameDebugConsole;

/// <summary>
/// Manages all of the flora in this region
/// </summary>
[CreateAssetMenu(fileName = "FloraMaster", menuName = "Game Logic/Flora Master")]
[System.Serializable]
public class FloraMaster : SerializedScriptableSingleton<FloraMaster>, IStreamer
{
    [OdinSerialize]
    private System.Guid _Guid;
    [Space(5)]
    [SerializeField]
    private GameObject _FloraPrefab;
    [OdinSerialize]
    private Dictionary<string, FloraInfo> _FloraDictionary = new Dictionary<string, FloraInfo>();

    [System.NonSerialized]
    private List<Flora> _Florae = new List<Flora>();

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        GameManager.AddStreamer(Instance);
        SceneManager.sceneLoaded += Instance.OnSceneLoaded;

        DebugLogConsole.AddCommandInstance("flora.grow", "Grows flora", nameof(Instance.DebugNotify), Instance);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (Flora flora in _Florae)
        {
            if (scene.buildIndex == flora.FloraData.SceneIndex)
            {
                Cell cell = GameGrid.Instance.GetCellLocal(flora.FloraData.CellPosition);
                cell.Occupied = false;

                Create(flora, cell);
            }
        }
    }

    public bool Add(string floraName)
    {
        Cell cell = GameGrid.Instance.HoveredCell;

        if (cell == null || cell.Occupied || cell.Type != CellType.Dirt)
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
        Cell cell = GameGrid.Instance.HoveredCell;

        if (cell == null || cell.Occupied || cell.Type != CellType.Dirt)
            return false;

        Flora flora = new Flora(floraInfo, cell.Local);

        if (!Create(flora, cell))
            return false;

        _Florae.Add(flora);

        return true;
    }

    public bool Create(Flora flora, Cell cell)
    {
        if (cell.Occupied)
            return false;

        GameObject obj = Instantiate(_FloraPrefab);

        FloraObject floraObject = obj.GetComponent<FloraObject>();
        floraObject.Initialize(flora);

        cell.Occupy(obj);

        return true;
    }

    public bool Remove(Flora flora)
    {
        return _Florae.Remove(flora);
    }

    public void Notify()
    {
        for (int i = _Florae.Count - 1; i >= 0; --i)
        {
            _Florae[i].Grow();
        }
    }
    public void DebugNotify()
    {
        for (int i = _Florae.Count - 1; i >= 0; --i)
        {
            _Florae[i].ForceGrowth();
        }
    }

    public void UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        _Florae.ForEach(x =>
            streamables.Add(
                $"{x.Cell.Local} {x.FloraData.SceneIndex}", 
                ((IStreamable<object>)x).OnUpload()));

        GameManager.Stream.Load(_Guid, streamables);
    }
    public void Load()
    {
        _Florae.Clear();

        if (!GameManager.Stream.TryGet(_Guid, out Dictionary<string, object> streamables))
            return;

        foreach (KeyValuePair<string, object> item in streamables)
        {
            if (!streamables.TryGetValue(item.Key, out object value))
                continue;

            Flora.Data data = value as Flora.Data;

            Flora flora = new Flora(_FloraDictionary[data.Name.ToLower()], data.CellPosition);
            flora.OnLoad(data);

            _Florae.Add(flora);
        }
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();

        _Florae.ForEach(x => x.OnBeforeSaving());
    }
}
