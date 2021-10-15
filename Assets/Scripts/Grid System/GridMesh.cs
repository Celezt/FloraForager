using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using MyBox;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class GridMesh : MonoBehaviour
{
    [SerializeField]
    private int _TexTilesPerRow, _TexTilesPerCol;

    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;
    private MeshCollider _MeshCollider;

    public Mesh Mesh => _MeshFilter.sharedMesh;

    public Vector3[] Vertices => Mesh.vertices;
    public int[] Triangles => Mesh.triangles;
    public Vector2[] UVs => Mesh.uv;

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
    }

#if UNITY_EDITOR
    [SerializeField, HideInInspector]
    private TempCellData[] _CellData;

    [Button]
    private void Clear()
    {
        Mesh.Clear();
        _CellData = null;
    }

    /// <summary>
    /// Compile cells into one mesh
    /// </summary>
    public void Compile(CellMesh[] cells)
    {
        Decompile();

        Vector3[] vertices = new Vector3[cells.Length * 4];
        int[] triangles = new int[cells.Length * 6];
        Vector2[] uvs = new Vector2[cells.Length * 4];

        _CellData = new TempCellData[cells.Length];

        for (int i = cells.Length - 1, v = 0, t = 0; i >= 0; --i, v += 4, t += 6)
        {
            CellMesh cell = cells[i];

            triangles[t + 0] = v;
            triangles[t + 1] = triangles[t + 4] = v + 1;
            triangles[t + 2] = triangles[t + 3] = v + 2;
            triangles[t + 5] = v + 3;

            Vector3[] tempVertices = cell.GetWorldVertices();

            vertices[v + 0] = tempVertices[0];
            vertices[v + 1] = tempVertices[1];
            vertices[v + 2] = tempVertices[2];
            vertices[v + 3] = tempVertices[3];

            Vector2[] tempUVs = cell.UVs;

            uvs[v + 0] = tempUVs[0];
            uvs[v + 1] = tempUVs[1];
            uvs[v + 2] = tempUVs[2];
            uvs[v + 3] = tempUVs[3];

            _CellData[i] = new TempCellData(cell.Type);

            DestroyImmediate(cell.gameObject);
        }

        _MeshFilter.mesh = new Mesh();
        Mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Mesh.vertices = vertices;
        Mesh.triangles = triangles;
        Mesh.uv = uvs;
        Mesh.RecalculateNormals();
    }

    /// <summary>
    /// Decompile current mesh into cells
    /// </summary>
    public CellMesh[] Decompile()
    {
        if (_CellData == null || _CellData.Length == 0)
            return null;

        CellMesh[] cells = new CellMesh[_CellData.Length];
        for (int i = 0; i < cells.Length; ++i)
        {
            Vector3 sum = Vector3.zero;
            for (int j = 0; j < 4; ++j)
                sum += Vertices[i * 4 + j];
            sum /= 4;

            GameObject cellObject = new GameObject($"Cell ({i})", typeof(CellMesh));
            cellObject.transform.position = sum;

            CellMesh mesh = cellObject.GetComponent<CellMesh>();

            mesh.Terrain = _MeshRenderer.sharedMaterial;
            mesh.Initialize(Vector2Int.one, _CellData[i].Type);

            cells[i] = mesh;
        }

        Clear();

        return cells;
    }

    [System.Serializable]
    private class TempCellData
    {
        public CellType Type;

        public TempCellData(CellType type)
        {
            Type = type;
        }
    }
#endif
}
