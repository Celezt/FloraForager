using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class GridMesh : MonoBehaviour
{
    [SerializeField, HideInInspector]
    public CellData[] CellsData;

    [SerializeField]
    private int _TexTilesPerRow, _TexTilesPerCol;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;
    private MeshCollider _MeshCollider;

    public Mesh Mesh => _MeshFilter.sharedMesh;

    public Vector3[] Vertices { get; private set; }
    public int[] Triangles { get; private set; }
    public Vector2[] UVs { get; private set; }

    public int TexTilesPerRow => _TexTilesPerRow;
    public int TexTilesPerCol => _TexTilesPerCol;

    private void OnValidate()
    {
        if (_MeshFilter == null)
        {
            _MeshFilter = GetComponent<MeshFilter>();
            _MeshRenderer = GetComponent<MeshRenderer>();
            _MeshCollider = GetComponent<MeshCollider>();
        }
    }

    private void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        _MeshCollider = GetComponent<MeshCollider>();

        Vertices = Mesh.vertices;
        Triangles = Mesh.triangles;
        UVs = Mesh.uv;

        if (!Application.isEditor)
            _MeshRenderer.enabled = false;
    }

#if UNITY_EDITOR

    [Button]
    private void Clear()
    {
        Mesh.Clear();
        CellsData = null;
    }

    /// <summary>
    /// Compile cells into one mesh
    /// </summary>
    public void Compile(CellMesh[] cells)
    {
        CellMesh[] decompiledCells = Decompile();

        if (decompiledCells != null)
            cells = cells.Concat(decompiledCells).ToArray();

        Vector3[] vertices = new Vector3[cells.Length * 4];
        int[] triangles = new int[cells.Length * 6];
        Vector2[] uvs = new Vector2[cells.Length * 4];

        CellsData = new CellData[cells.Length];

        for (int i = cells.Length - 1, v = i * 4, t = i * 6; i >= 0; --i, v -= 4, t -= 6)
        {
            CellMesh cell = cells[i];

            triangles[t + 0] = v;
            triangles[t + 1] = triangles[t + 4] = v + 1;
            triangles[t + 2] = triangles[t + 3] = v + 2;
            triangles[t + 5] = v + 3;

            Vector3[] tempVertices = cell.GetWorldVertices();

            vertices[v + 0] = tempVertices[0] - transform.position;
            vertices[v + 1] = tempVertices[1] - transform.position;
            vertices[v + 2] = tempVertices[2] - transform.position;
            vertices[v + 3] = tempVertices[3] - transform.position;

            Vector2[] tempUVs = cell.UVs;

            uvs[v + 0] = tempUVs[0];
            uvs[v + 1] = tempUVs[1];
            uvs[v + 2] = tempUVs[2];
            uvs[v + 3] = tempUVs[3];

            CellsData[i] = cell.Data;

            DestroyImmediate(cell.gameObject);
        }

        _MeshFilter.mesh = new Mesh();
        Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Mesh.vertices = vertices;
        Mesh.triangles = triangles;
        Mesh.uv = uvs;
        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();

        _MeshCollider.sharedMesh = Mesh; 
    }

    /// <summary>
    /// Decompile current mesh into cells
    /// </summary>
    public CellMesh[] Decompile()
    {
        if (CellsData == null || CellsData.Length == 0)
            return null;

        CellMesh[] cells = new CellMesh[CellsData.Length];
        for (int i = cells.Length - 1; i >= 0; --i)
        {
            Vector3 sum = Vector3.zero;
            for (int j = 0; j < 4; ++j)
                sum += Mesh.vertices[i * 4 + j];
            sum /= 4;

            GameObject cellObject = new GameObject($"Cell ({i})", typeof(CellMesh));
            cellObject.transform.position = sum + transform.position;

            CellMesh mesh = cellObject.GetComponent<CellMesh>();

            mesh.GridMaterial = _MeshRenderer.sharedMaterial;
            mesh.Initialize(CellsData[i]);

            cells[i] = mesh;
        }

        Clear();

        return cells;
    }
#endif
}
