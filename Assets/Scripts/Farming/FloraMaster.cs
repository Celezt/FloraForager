using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct KeyValuePair<Key, Value>
{
    public Key key;
    public Value value;
}

public class FloraMaster : MonoBehaviour
{
    [SerializeField] private List<KeyValuePair<string, GameObject>> _FloraPrefabs;

    private Dictionary<string, GameObject> _PrefabsDictionary;
    private List<Flora> _Floras;

    private void Awake()
    {
        _Floras = new List<Flora>();
        _PrefabsDictionary = _FloraPrefabs.ToDictionary(key => key.key, value => value.value);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    /// <summary>
    /// add a flora at currently selected tile based on its name
    /// </summary>
    public bool AddFlora(string name)
    {
        if (GridInteraction.CurrentTile == null)
            return false;

        GameObject flora;
        if ((flora = _PrefabsDictionary[name]) == null)
            return false;

        GameObject obj = Instantiate(flora);

        if (!GridInteraction.PlaceObject(GridInteraction.CurrentTile, obj))
        {
            Destroy(obj);
            return false;
        }

        return true;
    }

    public void Notify()
    {
        _Floras.ForEach(f => f.Grow());
    }
}
