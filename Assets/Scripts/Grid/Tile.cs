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
    private Vector2 _Position;
    private float _Size;

    public GameObject HeldObject => _HeldObject;
    public TileType TileType => _TileType;
    public Vector2 Position => _Position;
    public float Size => _Size;

    public bool Occupied { get; set; }

    public Tile(Vector2 position, float size, TileType tileType = TileType.Empty)
    {
        _Position = position;
        _Size = size;
        _TileType = tileType;

        Occupied = false;
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
