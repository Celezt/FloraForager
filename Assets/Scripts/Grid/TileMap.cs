using System;
using UnityEngine;

public class TileMap
{
    private Grid _Grid;
    private Tile[] _Tiles;

    public Tile[] Tiles => _Tiles;

    public TileMap(Grid grid, TileType tileType, bool random)
    {
        _Grid = grid;

        CreateTiles(tileType, random);
        AddNeighbours();
    }

    private void CreateTiles(TileType tileType, bool random)
    {
        _Tiles = new Tile[_Grid.Width * _Grid.Height];

        for (int x = 0; x < _Grid.Width; ++x)
        {
            for (int z = 0; z < _Grid.Height; ++z)
            {
                Vector3 posWorld = _Grid.transform.position + new Vector3(x * _Grid.TileSize, 0, z * _Grid.TileSize);
                Vector3 posLocal = new Vector3(x, 0, z);

                TileType tile = (random) ? (TileType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TileType)).Length) : tileType;

                _Tiles[x + z * _Grid.Width] = new Tile(posWorld, posLocal, _Grid.TileSize, tile);
            }
        }
    }

    private void AddNeighbours()
    {
        for (int x = 0; x < _Grid.Width; ++x)
        {
            for (int z = 0; z < _Grid.Height; ++z)
            {
                for (int i = -1; i <= 1; ++i)
                {
                    for (int j = -1; j <= 1; ++j)
                    {
                        if (j == 0 && i == 0)
                            continue;

                        if (WithinMap(x + j, z + i))
                        {
                            Tile tile = GetTile(x, z);
                            Tile neighbour = GetTile(x + j, z + i);

                            tile.AddNeighbour(neighbour);
                        }
                    }
                }
            }
        }
    }

    public Tile GetTile(int x, int y)
    {
        if (!WithinMap(x, y))
            return null;

        return _Tiles[x + y * _Grid.Width];
    }

    private bool WithinMap(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= _Grid.Width || y >= _Grid.Height);
    }
}
