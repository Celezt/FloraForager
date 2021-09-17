using UnityEngine;

public enum TileType
{
    Undefined,
    Empty,
    Dirt,
    Water,
}

public class Tile
{
    private GameObject _HeldObject;
    private TileType _TileType;
    private Vector3 _PositionWorld;
    private Vector3 _PositionLocal;
    private float _Size;

    public GameObject HeldObject => _HeldObject;
    public TileType TileType => _TileType;
    public Vector3 PositionWorld => _PositionWorld;
    public Vector3 PositionLocal => _PositionLocal;
    public Vector3 Middle => _PositionWorld + new Vector3(_Size / 2, 0, _Size / 2);
    public float Size => _Size;

    public bool Occupied { get; set; }

    public Tile(Vector3 posW, Vector3 posL, float size, TileType tileType = TileType.Empty)
    {
        _PositionWorld = posW;
        _PositionLocal = posL;
        _Size = size;
        _TileType = tileType;

        Occupied = false;
    }

    public bool UpdateType(TileType type)
    {
        if (Occupied)
            return false;

        _TileType = type;

        return true;
    }

    public bool OccupyTile(GameObject obj)
    {
        if (Occupied) 
            return false;

        _HeldObject = obj;
        Occupied = true;

        return true;
    }

    public GameObject FreeTile()
    {
        if (!Occupied)
            return null;

        Occupied = false;
        return _HeldObject;
    }
}
