using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Grid))]
public class GridInteraction : MonoBehaviour
{
    private Grid _Grid;

    private Tile _CurrentTile;
    private Vector3 _CurrentTilePos;

    public Tile CurrentTile => _CurrentTile;

    private void Awake()
    {
        _Grid = GetComponent<Grid>();

        
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Grid")))
        {
            int x = Mathf.FloorToInt((hitInfo.point.x - transform.position.x + _Grid.TileSize / 2) / _Grid.TileSize);
            int z = Mathf.FloorToInt((hitInfo.point.z - transform.position.z + _Grid.TileSize / 2) / _Grid.TileSize);

            _CurrentTilePos.x = x;
            _CurrentTilePos.y = hitInfo.point.y;
            _CurrentTilePos.z = z;

            _CurrentTile = _Grid.TileMap.GetTile(x, z);
        }
        else
        {
            _CurrentTile = null;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            cube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            cube.transform.position = transform.position + _CurrentTilePos * _Grid.TileSize;
            cube.transform.position += new Vector3(0.0f, cube.transform.lossyScale.y / 2.0f, 0.0f);

            if (!PlaceObject(cube))
                Destroy(cube);
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            RemoveObject();
        }
    }

    /// <summary>
    /// place an object at currently hovered tile
    /// </summary>
    /// <param name="obj">object to be placed</param>
    /// <returns>returns false if object cannot be placed</returns>
    public bool PlaceObject(GameObject obj)
    {
        if (_CurrentTile == null)
            return false;

        if (!_CurrentTile.OccupyTile(obj))
            return false;

        return true;
    }

    public bool RemoveObject()
    {
        if (_CurrentTile == null)
            return false;

        Destroy(_CurrentTile.FreeTile());

        return true;
    }
}
