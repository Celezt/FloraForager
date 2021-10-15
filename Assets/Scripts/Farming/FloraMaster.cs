using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

/// <summary>
/// Manages all of the flora in this region
/// </summary>
public class FloraMaster : Singleton<FloraMaster>
{
    [SerializeField] private GameObject _FloraPrefab;
    [SerializeField] private List<FloraData> _FloraVariants;

    private static Dictionary<string, FloraData> _FloraDictionary;
    private static List<Flora> _Florae = new List<Flora>();

    private void Awake()
    {
        _FloraDictionary = _FloraVariants.ToDictionary(key => key.Name.ToLower(), value => value);
    }

    /// <summary>
    /// creates a flora at currently selected tile based on its name
    /// </summary>
    public bool Add(string name)
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell == null || cell.Type != CellType.Dirt)
            return false;

        string key = name.ToLower();

        if (!_FloraDictionary.ContainsKey(key))
            return false;

        Flora flora = new Flora(_FloraDictionary[key], cell);

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
}
