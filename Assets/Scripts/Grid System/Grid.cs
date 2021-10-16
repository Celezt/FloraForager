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

    private Vector3 _Position;
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

        for (int i = 0; i < _Mesh.Vertices.Length; i += 4)
        {
            Vector2Int pos = new Vector2Int(i % _Width, i / _Width);
            Vector3 worldPosition = _Mesh.Vertices[pos.x + pos.y * _Width];

            int x = Mathf.FloorToInt(worldPosition.x - transform.position.x - _Position.x);
            int z = Mathf.FloorToInt(worldPosition.z - transform.position.z - _Position.z);

            _Cells[x + z * _Width] = new Cell(this, _Mesh.CellsData[i / 4],
                worldPosition,
                new Vector2Int(x, z),
                new Vector2Int(1, 1), (i / 4));
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks))
        {
            int x = Mathf.FloorToInt(hitInfo.point.x - transform.position.x - _Position.x);
            int z = Mathf.FloorToInt(hitInfo.point.z - transform.position.z - _Position.z);

            HoveredCell = GetCell(x, z);
        }
        else
            HoveredCell = null;
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

    public void UpdateCellsUVs(CellType type, params Cell[] cells)
    {
        foreach (Cell cell in cells)
        {
            cell.SetType(type);

            float tileTexProcRow = 1.0f / _Mesh.TexTilesPerRow;
            float tileTexProcCol = 1.0f / _Mesh.TexTilesPerCol;

            float proc = (int)cell.Data.Type / (float)_Mesh.TexTilesPerRow;
            float dFix = 0.00f; // dilation

            _Mesh.UVs[cell.MeshIndex * 4 + 0] = new Vector2(proc + dFix, 0.0f + dFix); // bottom-left
            _Mesh.UVs[cell.MeshIndex * 4 + 1] = new Vector2(proc + dFix, 1.0f - dFix); // top-left
            _Mesh.UVs[cell.MeshIndex * 4 + 2] = new Vector2(proc + tileTexProcRow - dFix, 0.0f + dFix); // bottom-right
            _Mesh.UVs[cell.MeshIndex * 4 + 3] = new Vector2(proc + tileTexProcRow - dFix, 1.0f - dFix); // top-right
        }

        _MeshFilter.mesh.uv = _Mesh.UVs;
    }

    private (int, int) FindBoundaries()
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(-float.MaxValue, -float.MaxValue);

        for (int i = 0; i < _Mesh.Vertices.Length; ++i)
        {
            Vector3 vertex = _Mesh.Vertices[i];

            if (vertex.x < min.x)
                min.x = vertex.x;
            if (vertex.z < min.y)
                min.y = vertex.z;
            if (vertex.x > max.x)
                max.x = vertex.x;
            if (vertex.z > max.y)
                max.y = vertex.z;
        }

        if (min.x == float.MaxValue ||
            min.y == float.MaxValue ||
            max.x == -float.MaxValue ||
            max.y == -float.MaxValue)
            return (0, 0);

        _Position = new Vector3(min.x, 0, min.y);

        (int, int) boundary = (
            Mathf.Abs(Mathf.FloorToInt(max.x - min.x)),
            Mathf.Abs(Mathf.FloorToInt(max.y - min.y)));

        return boundary;
    }
}
