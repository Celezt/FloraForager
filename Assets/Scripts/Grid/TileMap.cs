using System;
using UnityEngine;

public class TileMap
{
    private Tile[] _Tiles;

    private int _Width;
    private int _Height;

    public Tile[] Tiles => _Tiles;

    public TileMap(Grid grid, int width, int height, float tileSize)
    {
        _Width = width;
        _Height = height;

        _Tiles = new Tile[_Width * _Height];
        for (int x = 0; x < _Width; ++x)
        {
            for (int z = 0; z < _Height; ++z)
            {
                Vector3 posWorld = grid.transform.position + new Vector3(x * tileSize, 0, z * tileSize);
                Vector3 posLocal = new Vector3(x, 0, z);

                _Tiles[x + z * _Width] = new Tile(posWorld, posLocal, tileSize, (TileType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TileType)).Length));
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (!WithinMap(x, y))
            return null;

        return _Tiles[x + y * _Width];
    }

    private bool WithinMap(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= _Width || y >= _Height);
    }
}