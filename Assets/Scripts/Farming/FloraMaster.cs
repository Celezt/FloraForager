using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using Sirenix.Serialization;

/// <summary>
/// Manages all of the flora in this region
/// </summary>
[CreateAssetMenu(fileName = "FloraMaster", menuName = "Scriptable Objects/Flora Master")]
[System.Serializable]
public class FloraMaster : SerializedScriptableSingleton<FloraMaster>
{
    [SerializeField] 
    private GameObject _FloraPrefab;
    [SerializeField] 
    private System.Guid _guid;
    [OdinSerialize]
    private Dictionary<string, FloraData> _FloraDictionary = new Dictionary<string, FloraData>();
    private List<Flora> _Florae = new List<Flora>();

    private void Awake()
    {
        if (_guid == System.Guid.Empty)
            _guid = System.Guid.NewGuid();

        UpLoad();
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void Initialize()
    {
        FloraMaster.Instance.UpLoad();
    }
#endif

    /// <summary>
    /// creates a flora at currently selected tile based on its name
    /// </summary>
    public bool Add(string floraName)
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell == null)
            return false;

        if (cell.Data.Type != CellType.Dirt && cell.Data.Type != CellType.Soil)
            return false;

        string key = floraName.ToLower();

        if (!_FloraDictionary.ContainsKey(key))
            return false;

        Flora flora = new Flora(_FloraDictionary[key], cell);

        if (!Create(flora, cell))
            return false;

        _Florae.Add(flora);

        return true;
    }

    /// <summary>
    /// creates a flora at currently selected tile based on its data
    /// </summary>
    public bool Add(FloraData floraData)
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell == null)
            return false;

        if (cell.Data.Type != CellType.Dirt && cell.Data.Type != CellType.Soil)
            return false;

        Flora flora = new Flora(floraData, cell);

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

    public object UpLoad()
    {
        Dictionary<Vector2Int, object> streamables = new Dictionary<Vector2Int, object>();

        _Florae.ForEach(x => streamables.Add(x.Cell.Local, ((IStreamable<object>)x).OnUpload()));

        GameManager.Stream.Load(_guid, streamables);

        return null;
    }

    public void Load()
    {
        foreach (IStreamable<object> stream in _Florae)
        {
            string typeName = stream.GetType().ToString();

            Dictionary<string, object> streamables = (Dictionary<string, object>)GameManager.Stream.Get(_guid);

            if (streamables.TryGetValue(typeName, out object value))
                stream.OnLoad(value);
        }
    }
}
