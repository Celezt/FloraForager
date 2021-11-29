using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Only used by grid tool to create grid mesh
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CellMesh : MonoBehaviour
{
    public CellData Data = new CellData(null, CellType.Empty);

    [Space(10)]
    public Material GridMaterial;

    public Vector2Int Size { get; private set; } = Vector2Int.one;

    public Vector3[] Vertices { get; private set; }
    public int[] Triangles { get; private set; }
    public Vector2[] UVs { get; private set; }

    private GridMesh _GridMesh;

    private Mesh _Mesh;
    private MeshFilter _MeshFilter;
    private MeshRenderer _MeshRenderer;

    private void Awake()
    {
        Initialize(Data);
    }

    private void Update()
    {
        // snap to grid
        transform.position = new Vector3(
            Mathf.FloorToInt(transform.position.x), 
            transform.position.y, 
            Mathf.FloorToInt(transform.position.z));

        GameObject heldObject = Data.HeldObject;

        if (heldObject != null)
        {
            heldObject.transform.position = gameObject.transform.position;
            if (heldObject.TryGetComponent(out MeshFilter meshFilter))
                heldObject.transform.position += Vector3.up * meshFilter.sharedMesh.bounds.extents.y;
        }
    }

    public Vector3[] GetWorldVertices()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = transform.position + Vertices[0];
        vertices[1] = transform.position + Vertices[1];
        vertices[2] = transform.position + Vertices[2];
        vertices[3] = transform.position + Vertices[3];
        return vertices;
    }

    public void SetSize(Vector2Int size)
    {
        Size = size;

        Vertices[0] = new Vector3(-Size.x / 2f, 0f, -Size.y / 2f);
        Vertices[1] = new Vector3(-Size.x / 2f, 0f,  Size.y / 2f);
        Vertices[2] = new Vector3( Size.x / 2f, 0f, -Size.y / 2f);
        Vertices[3] = new Vector3( Size.x / 2f, 0f,  Size.y / 2f);

        BuildMesh();
    }

    public void SetType(CellType type)
    {
        Data.Type = type;

        float tileTexProcRow = 1.0f / _GridMesh.TexTilesPerRow;
        float tileTexProcCol = 1.0f / _GridMesh.TexTilesPerCol;

        float proc = (int)type / (float)_GridMesh.TexTilesPerRow;
        float dFix = 0.00f; // dilation

        UVs[0] = new Vector2(proc + dFix,                  0.0f + dFix); // bottom-left
        UVs[1] = new Vector2(proc + dFix,                  1.0f - dFix); // top-left
        UVs[2] = new Vector2(proc + tileTexProcRow - dFix, 0.0f + dFix); // bottom-right
        UVs[3] = new Vector2(proc + tileTexProcRow - dFix, 1.0f - dFix); // top-right

        BuildMesh();
    }

    public void Initialize(CellData data)
    {
        Data = data;

        _GridMesh = GameGrid.Instance.transform.GetComponent<GridMesh>();

        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();

        _MeshRenderer.material = GridMaterial;

        _Mesh = new Mesh();
        _MeshFilter.mesh = _Mesh;

        Vertices = new Vector3[4];
        Triangles = new int[6];
        UVs = new Vector2[4];

        Triangles[0] = 0;
        Triangles[1] = Triangles[4] = 1;
        Triangles[2] = Triangles[3] = 2;
        Triangles[5] = 3;

        SetSize(Size);
        SetType(Data.Type);
    }

    private void BuildMesh()
    {
        _Mesh.Clear(false);
        _Mesh.vertices = Vertices;
        _Mesh.triangles = Triangles;
        _Mesh.uv = UVs;
        _Mesh.RecalculateNormals();
    }
}
#endif
