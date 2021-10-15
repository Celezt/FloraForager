using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using MyBox;

public class Grid : Singleton<Grid> // One grid per level
{
    [SerializeField]
    private LayerMask _LayerMasks;

    private Cell[] _Cells;

    private GridMesh _Mesh;
    private MeshFilter _MeshFilter;

    private Vector3Int _Position;
    private int _Width, _Height;

    public Cell HoveredCell { get; private set; }

    private void Awake()
    {
        _Mesh = GetComponent<GridMesh>();
        _MeshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        (int, int) boundaries = FindBoundaries();

        _Width = boundaries.Item1;
        _Height = boundaries.Item2;

        _Cells = new Cell[_Width * _Height];

        for (int x = 0; x < _Width; ++x)
        {
            for (int z = 0; z < _Height; ++z)
            {
                int i = x + z * _Width;

                _Cells[i] = new Cell(this, CellType.Empty, 
                    _Position + new Vector3Int(0, (int)_Mesh.Vertices[i / 4].y, 0), 
                    new Vector2Int(x, z), 
                    new Vector2Int(1, 1));
            }
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks))
        {
            int x = Mathf.FloorToInt(hitInfo.point.x - transform.position.x);
            int z = Mathf.FloorToInt(hitInfo.point.z - transform.position.z);

            HoveredCell = GetCell(x, z);
        }
        else
        {
            HoveredCell = null;
        }
    }

    /// <summary>
    /// place an object at specified tile
    /// </summary>
    /// <param name="gameObject">object to be placed</param>
    /// <returns>false if object cannot be placed</returns>
    public bool OccupyCell(Cell cell, GameObject gameObject)
    {
        if (cell == null)
            return false;

        if (!cell.Occupy(gameObject))
            return false;

        gameObject.transform.position = cell.Middle;

        return true;
    }

    /// <summary>
    /// frees object from cell, does not destroy
    /// </summary>
    public GameObject FreeCell(Cell cell)
    {
        if (cell == null)
            return null;

        return cell.Free();
    }

    public Cell GetCell(int x, int z)
    {
        if (!WithinGrid(x, z))
            return null;

        return _Cells[x + z * _Width];
    }

    public bool WithinGrid(int x, int z)
    {
        return !(x < 0 || z < 0 || x >= _Width || z >= _Height);
    }

    private (int, int) FindBoundaries()
    {
        Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int max = new Vector2Int(-int.MaxValue, -int.MaxValue);

        for (int i = 0; i < _Mesh.Vertices.Length; ++i)
        {
            Vector3Int vertex = Vector3Int.FloorToInt(_Mesh.Vertices[i]);

            if (vertex.x < min.x)
                min.x = vertex.x;
            if (vertex.y < min.y)
                min.y = vertex.y;
            if (vertex.x > max.x)
                max.x = vertex.x;
            if (vertex.y > max.y)
                max.y = vertex.y;
        }

        if (min.x == int.MaxValue ||
            max.x == -int.MaxValue ||
            min.y == int.MaxValue ||
            max.y == -int.MaxValue)
            return (0, 0);

        _Position = new Vector3Int(min.x, 0, min.y);

        (int, int) boundary = (
            Mathf.Abs(max.x - min.x), 
            Mathf.Abs(max.y - min.y));

        return boundary;
    }

    public void UpdateCellsUVs(CellType type, params Cell[] cells)
    {
        foreach (Cell cell in cells)
        {
            cell.SetType(type);

            int i = (int)cell.Local.x + (int)cell.Local.y * _Height;

            float tileTexProcRow = 1.0f / _Mesh.TexTilesPerRow;
            float tileTexProcCol = 1.0f / _Mesh.TexTilesPerCol;

            float proc = (int)cell.Type / (float)_Mesh.TexTilesPerRow;
            float dFix = 0.00f; // dilation

            _Mesh.UVs[i * 4 + 0] = new Vector2(proc + dFix, 0.0f + dFix); // bottom-left
            _Mesh.UVs[i * 4 + 1] = new Vector2(proc + dFix, 1.0f - dFix); // top-left
            _Mesh.UVs[i * 4 + 2] = new Vector2(proc + tileTexProcRow - dFix, 0.0f + dFix); // bottom-right
            _Mesh.UVs[i * 4 + 3] = new Vector2(proc + tileTexProcRow - dFix, 1.0f - dFix); // top-right
        }

        _MeshFilter.mesh.uv = _Mesh.UVs;
    }
}
