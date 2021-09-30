using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Undefined,
    Empty,
    Dirt,
    Soil,
    Water,
}

public class Tile
{
    private TileMap _TileMap;       // which tilemap this belongs to
    private GameObject _HeldObject; // which object is being held by this tile
    private TileType _TileType;
    private Vector3 _PositionWorld;
    private Vector3 _PositionLocal;
    private float _Size;

    private List<Tile> _Neighbours;

    public TileMap TileMap => _TileMap;
    public GameObject HeldObject => _HeldObject;
    public TileType TileType => _TileType;
    public Vector3 PositionWorld => _PositionWorld;
    public Vector3 PositionLocal => _PositionLocal;
    public Vector3 Middle => _PositionWorld + new Vector3(_Size / 2, 0, _Size / 2);
    public float Size => _Size;

    public List<Tile> Neighbours => _Neighbours;

    public bool Occupied { get; set; }

    public Tile(TileMap tileMap, Vector3 posW, Vector3 posL, float size, TileType tileType = TileType.Empty)
    {
        _TileMap = tileMap;
        _PositionWorld = posW;
        _PositionLocal = posL;
        _Size = size;
        _TileType = tileType;

        _Neighbours = new List<Tile>(8);
        Occupied = false;
    }

    public bool UpdateType(TileType type)
    {
        _TileType = type;
        return true;
    }

    public bool OccupyTile(GameObject obj)
    {
        if (Occupied) 
            return false;

        _HeldObject = obj;

        return (Occupied = true);
    }

    public GameObject FreeTile()
    {
        if (!Occupied)
            return null;

        Occupied = false;

        return _HeldObject;
    }

    public void AddNeighbour(Tile tile)
    {
        _Neighbours.Add(tile);
    }
    public void RemoveNeighbour(Tile tile)
    {
        _Neighbours.Remove(tile);
    }
}
