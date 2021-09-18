using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Grid : MonoBehaviour
{
    [Space(5), Header("Grid")] 
    [SerializeField] private int _Width = 100;
    [SerializeField] private int _Height = 50;
    [SerializeField] private float _TileSize = 1.0f;

    [Space(5), Header("Texture")]
    [SerializeField] private Texture2D _TerrainTiles;
    [SerializeField] private int _TexTilesPerRow;
    [SerializeField] private int _TexTilesPerCol;

    [Space(5), Header("Generation")]
    [SerializeField] private bool _Random;
    [SerializeField] private TileType _TileType;
    
    private TileMap _TileMap;

    private Vector2[] _Uvs;

    public TileMap TileMap => _TileMap;
    public int Width => _Width;
    public int Height => _Height;
    public float TileSize => _TileSize;

    public int TexTilesPerRow => _TexTilesPerRow;
    public int TexTilesPerCol => _TexTilesPerCol;

    public Vector2[] Uvs => _Uvs;

    private void Awake()
    {
        BuildMesh();
    }

    private void Update()
    {

    }

    public void BuildMesh()
    {
        _TileMap = new TileMap(this, _TileType, _Random);

        int vertCount = _Width * _Height * 4;

        float tileTexProcRow = 1.0f / _TexTilesPerRow;
        float tileTexProcCol = 1.0f / _TexTilesPerCol;

        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];

        int[] triangles = new int[_Width * _Height * 6];

        float vertexOffset = _TileSize;

        int v = 0, t = 0;
        for (int x = 0; x < _Width; ++x)
        {
            for (int z = 0; z < _Height; z++)
            {
                Tile tile = _TileMap.Tiles[x + z * _Width];

                Vector3 tileOffset = new Vector3(x * _TileSize, 0, z * _TileSize);

                vertices[v + 0] = new Vector3( 0,             0,             0)            + tileOffset;
                vertices[v + 1] = new Vector3( 0,             0,             vertexOffset) + tileOffset;
                vertices[v + 2] = new Vector3( vertexOffset,  0,             0)            + tileOffset;
                vertices[v + 3] = new Vector3( vertexOffset,  0,             vertexOffset) + tileOffset;

                normals[v + 0] = Vector3.up;
                normals[v + 1] = Vector3.up;
                normals[v + 2] = Vector3.up;
                normals[v + 3] = Vector3.up;

                float proc = (int)tile.TileType / (float)_TexTilesPerRow;
                float dFix = 0.05f; // dilation

                uv[v + 0] = new Vector2(proc + dFix,                  1.0f - dFix); // top-left
                uv[v + 1] = new Vector2(proc + tileTexProcRow - dFix, 1.0f - dFix); // top-right
                uv[v + 2] = new Vector2(proc + dFix,                  0.0f + dFix); // bottom-left
                uv[v + 3] = new Vector2(proc + tileTexProcRow - dFix, 0.0f + dFix); // bottom-right

                triangles[t] = v;
                triangles[t + 1] = triangles[t + 4] = v + 1;
                triangles[t + 2] = triangles[t + 3] = v + 2;
                triangles[t + 5] = v + 3;

                v += 4;
                t += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = _Uvs = uv;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        meshRenderer.sharedMaterial.mainTexture = _TerrainTiles;

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}
