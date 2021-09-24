using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FloraMaster : MonoBehaviour
{
    [SerializeField] private List<GameObject> _FloraPrefabs;

    private Dictionary<string, GameObject> _PrefabsDictionary;
    private List<Flora> _Floras;

    private void Awake()
    {
        _Floras = new List<Flora>();
        _PrefabsDictionary = _FloraPrefabs.ToDictionary(key => key.GetComponent<Flora>().Name, value => value);
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            AddFlora("null");
        }
    }

    /// <summary>
    /// add a flora at currently selected tile based on its name
    /// </summary>
    public bool AddFlora(string name)
    {
        Tile tile = GridInteraction.CurrentTile;

        if (tile == null)
            return false;

        GameObject prefab;
        if ((prefab = _PrefabsDictionary[name]) == null)
            return false;

        GameObject obj = Instantiate(prefab);

        if (!GridInteraction.PlaceObject(tile, obj))
        {
            Destroy(obj);
            return false;
        }

        Flora flora = obj.GetComponent<Flora>();
        flora.Initialize(this, tile);

        _Floras.Add(flora);

        return true;
    }

    public bool RemoveFlora(Flora flora)
    {
        if (!_Floras.Contains(flora))
            return false;

        _Floras.Remove(flora);

        return true;
    }

    public void Notify()
    {
        _Floras.ForEach(f => f.Grow());
    }
}
