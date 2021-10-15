using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private Grid _Grid; // associated grid
    private List<Cell> _Neighbours;

    private GameObject _HeldObject;

    // properties
    private CellType _Type;
    private Vector3Int _PositionWorld; // position in the world
    private Vector2Int _PositionLocal; // position relative to grid
    private Vector2Int _Size;

    public bool Occupied;

    public Grid Grid => _Grid;
    public List<Cell> Neighbours => _Neighbours;

    public GameObject HeldObject => _HeldObject;

    public CellType Type => _Type;
    public Vector3Int World => _PositionWorld;
    public Vector2Int Local => _PositionLocal;
    public Vector2Int Size => _Size;
    public Vector2 Middle => _PositionWorld + new Vector3(_Size.x / 2.0f, 0, _Size.y / 2.0f);

    public Cell(Grid grid, CellType type, Vector3Int posW, Vector2Int posL, Vector2Int size)
    {
        _Grid = grid;
        _Type = type;
        _PositionWorld = posW;
        _PositionLocal = posL;
        _Size = size;
    }

    public bool Occupy(GameObject gameObject)
    {
        if (Occupied)
            return false;

        _HeldObject = gameObject;

        return (Occupied = true);
    }

    public GameObject Free()
    {
        if (!Occupied)
            return null;

        Occupied = false;

        return _HeldObject;
    }

    public void SetType(CellType type)
    {
        _Type = type;
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
