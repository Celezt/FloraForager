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

    private Dictionary<string, FloraData> _VariantsDictionary;
    private List<FloraObject> _FloraObjects;

    private void Awake()
    {
        _FloraObjects = new List<FloraObject>();
        _VariantsDictionary = _FloraVariants.ToDictionary(key => key.Name.ToLower(), value => value);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!AddFlora("Variant"))
            {
                if (GridInteraction.CurrentTile != null && GridInteraction.CurrentTile.HeldObject != null)
                {
                    GridInteraction.CurrentTile.HeldObject.TryGetComponent(out FloraObject flora);

                    if (flora != null)
                    {
                        flora.Watered = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// add a flora at currently selected tile based on its name
    /// </summary>
    public bool AddFlora(string name)
    {
        Tile tile = GridInteraction.CurrentTile;

        if (tile == null || tile.TileType != TileType.Dirt)
            return false;

        string key = name.ToLower();

        if (!_VariantsDictionary.ContainsKey(key))
            return false;

        GameObject obj = Instantiate(_FloraPrefab);

        if (!GridInteraction.PlaceObject(tile, obj))
        {
            Destroy(obj);
            return false;
        }

        FloraObject floraObject = obj.GetComponent<FloraObject>();
        floraObject.Initialize(_VariantsDictionary[key], tile);

        _FloraObjects.Add(floraObject);

        return true;
    }

    public bool RemoveFlora(FloraObject floraObject)
    {
        if (!_FloraObjects.Contains(floraObject))
            return false;

        _FloraObjects.Remove(floraObject);

        return true;
    }

    public void Notify()
    {
        _FloraObjects.ForEach(f => f.Grow());
    }
}
