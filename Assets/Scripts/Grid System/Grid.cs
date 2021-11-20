using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using MyBox;

public class Grid : Singleton<Grid> // One grid per level
{
    [SerializeField]
    private float _MaxHitDistance = 10.0f;
    [SerializeField]
    private LayerMask _LayerMasks;

    private Cell[] _Cells;

    private GridMesh _GridMesh;
    private MeshFilter _MeshFilter;

    private Vector3 _Position;
    private int _Width, _Height;

    private GameObject _Player;

    public Cell HoveredCell { get; private set; }

    private void Awake()
    {
        _GridMesh = GetComponent<GridMesh>();
        _MeshFilter = GetComponent<MeshFilter>();

        (int, int) boundaries = FindBoundaries();

        _Width = boundaries.Item1;
        _Height = boundaries.Item2;

        _Cells = new Cell[_Width * _Height];

        for (int i = 0; i < _MeshFilter.mesh.vertices.Length; i += 4)
        {
            Vector2Int pos = new Vector2Int(i % _Width, i / _Width);
            Vector3 worldPosition = transform.TransformPoint(_MeshFilter.mesh.vertices[pos.x + pos.y * _Width]);

            int x = Mathf.FloorToInt(worldPosition.x - _Position.x);
            int z = Mathf.FloorToInt(worldPosition.z - _Position.z);

            _Cells[x + z * _Width] = new Cell(this, _GridMesh.CellsData[i / 4],
                worldPosition,
                new Vector2Int(x, z),
                new Vector2Int(1, 1), (i / 4));
        }

        AddNeighbours();

        _Player = PlayerInput.GetPlayerByIndex(0)?.gameObject;
    }

    private void Update()
    {
        if (_Player == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks);

        float distance = Mathf.Sqrt(
            Mathf.Pow(hitInfo.point.x - _Player.transform.position.x, 2) +
            Mathf.Pow(hitInfo.point.z - _Player.transform.position.z, 2));

        if (_Player != null && hit && distance <= _MaxHitDistance)
        {
            HoveredCell = GetCellWorld(hitInfo.point);
        }
        else
            HoveredCell = null;
    }

    public Cell GetCellLocal(int x, int z)
    {
        if (!WithinGrid(x, z))
            return null;

        return _Cells[x + z * _Width];
    }
    public Cell GetCellLocal(Vector2Int pos)
    {
        return GetCellLocal(pos.x, pos.y);
    }

    public Cell GetCellWorld(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x - _Position.x);
        int z = Mathf.FloorToInt(worldPos.z - _Position.z);

        return GetCellLocal(x, z);
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

            float tileTexProcRow = 1.0f / _GridMesh.TexTilesPerRow;
            float tileTexProcCol = 1.0f / _GridMesh.TexTilesPerCol;

            float proc = (int)cell.Type / (float)_GridMesh.TexTilesPerRow;
            float dFix = 0.00f; // dilation

            _GridMesh.UVs[cell.MeshIndex * 4 + 0] = new Vector2(proc + dFix, 0.0f + dFix); // bottom-left
            _GridMesh.UVs[cell.MeshIndex * 4 + 1] = new Vector2(proc + dFix, 1.0f - dFix); // top-left
            _GridMesh.UVs[cell.MeshIndex * 4 + 2] = new Vector2(proc + tileTexProcRow - dFix, 0.0f + dFix); // bottom-right
            _GridMesh.UVs[cell.MeshIndex * 4 + 3] = new Vector2(proc + tileTexProcRow - dFix, 1.0f - dFix); // top-right
        }

        _MeshFilter.mesh.uv = _GridMesh.UVs;
    }

    private (int, int) FindBoundaries()
    {
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(-float.MaxValue, -float.MaxValue);

        for (int i = 0; i < _MeshFilter.mesh.vertices.Length; ++i)
        {
            Vector3 vertex = transform.TransformPoint(_MeshFilter.mesh.vertices[i]);

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

    private void AddNeighbours()
    {
        for (int i = 0; i < _MeshFilter.mesh.vertices.Length; i += 4)
        {
            Vector2Int pos = new Vector2Int(i % _Width, i / _Width);
            Vector3 worldPosition = transform.TransformPoint(_MeshFilter.mesh.vertices[pos.x + pos.y * _Width]);

            int xPos = Mathf.FloorToInt(worldPosition.x - _Position.x);
            int zPos = Mathf.FloorToInt(worldPosition.z - _Position.z);

            Cell cell = _Cells[xPos + zPos * _Width];

            for (int x = -1; x <= 1; ++x)
            {
                for (int z = -1; z <= 1; ++z)
                {
                    if (!WithinGrid(xPos + x, zPos + z) || (x == 0 && z == 0))
                        continue;

                    Cell neighbour = _Cells[(xPos + x) + (zPos + z) * _Width];

                    if (neighbour != null)
                        cell.AddNeighbour(neighbour);
                }
            }
        }
    }
}
