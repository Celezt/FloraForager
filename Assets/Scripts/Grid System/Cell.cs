using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private GameGrid _Grid; // associated grid
    private List<Cell> _Neighbours;

    private CellData _Data;

    // properties
    private Vector3 _PositionWorld; // position in the world
    private Vector2Int _PositionLocal; // position relative to grid
    private Vector2Int _Size;
    private int _MeshIndex;

    public bool Occupied;

    public GameGrid Grid => _Grid;
    public List<Cell> Neighbours => _Neighbours;

    public GameObject HeldObject => _Data.HeldObject;
    public CellType Type => _Data.Type;

    public Vector3 World => _PositionWorld;
    public Vector2Int Local => _PositionLocal;
    public Vector2Int Size => _Size;
    public int MeshIndex => _MeshIndex;
    public Vector3 Middle => _PositionWorld + new Vector3(_Size.x / 2.0f, 0, _Size.y / 2.0f);

    public Cell(GameGrid grid, CellData data, Vector3 posW, Vector2Int posL, Vector2Int size, int meshIndex)
    {
        _Grid = grid;
        _Data = data;
        _PositionWorld = posW;
        _PositionLocal = posL;
        _Size = size;
        _MeshIndex = meshIndex;

        _Neighbours = new List<Cell>();

        Occupied = (data.HeldObject != null);
    }

    public bool Occupy(GameObject gameObject)
    {
        if (Occupied)
            return false;

        _Data.HeldObject = gameObject;

        gameObject.transform.position = Middle;

        return (Occupied = true);
    }

    public GameObject Free()
    {
        if (!Occupied)
            return null;

        Occupied = false;

        return _Data.HeldObject;
    }

    public void SetType(CellType type)
    {
        _Data.Type = type;
    }

    public void AddNeighbour(Cell cell)
    {
        _Neighbours.Add(cell);
    }
    public void RemoveNeighbour(Cell cell)
    {
        _Neighbours.Remove(cell);
    }
}
