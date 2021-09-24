using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Grid))]
public class GridInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _SelectionObject;

    private Grid _Grid;
    private MeshFilter _MeshFilter;

    private GameObject _Selection;
    private Vector3 _CurrentTilePos;
    private bool _MouseCollision; // if mouse is currently colliding with grid

    public static Tile CurrentTile { get; private set; } = null;
    public static GridInteraction CurrentGrid { get; private set; } = null;
    public bool MouseCollision => _MouseCollision;

    public int Priority => 0;

    private void Awake()
    {
        _Grid = GetComponent<Grid>();
        _MeshFilter = GetComponent<MeshFilter>();

        _Selection = Instantiate(_SelectionObject, Vector3.zero, Quaternion.identity);

        float scale = _Selection.transform.localScale.x;
        _Selection.transform.localScale = new Vector3(scale * _Grid.TileSize, scale, scale * _Grid.TileSize);
    }



    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Grid"));

        if (!collision)
        {
            CurrentTile = null;
            CurrentGrid = null;
        }

        if (_MouseCollision = (collision && hitInfo.transform.gameObject == gameObject))
        {
            int x = Mathf.FloorToInt((hitInfo.point.x - transform.position.x) / _Grid.TileSize);
            int z = Mathf.FloorToInt((hitInfo.point.z - transform.position.z) / _Grid.TileSize);

            _CurrentTilePos.x = x;
            _CurrentTilePos.y = hitInfo.point.y;
            _CurrentTilePos.z = z;

            CurrentTile = _Grid.TileMap.GetTile(x, z);
            CurrentGrid = this;
        }

        if (ValidTile(CurrentTile) && !CurrentTile.Occupied)
        {
            if (!_Selection.activeSelf)
                _Selection.SetActive(true);

            _Selection.transform.position = CurrentTile.Middle + new Vector3(0, float.Epsilon, 0.0f);
        }
        else
        {
            if (_Selection.activeSelf)
                _Selection.SetActive(false);

            _Selection.transform.position = Vector3.zero;
        }
    }

    /// <summary>
    /// place an object at specified tile
    /// </summary>
    /// <param name="obj">object to be placed</param>
    /// <returns>false if object cannot be placed</returns>
    public static bool PlaceObject(Tile tile, GameObject obj)
    {
        if (tile == null)
            return false;

        if (!tile.OccupyTile(obj))
            return false;

        obj.transform.position = tile.Middle;
        obj.transform.position += Vector3.up * (obj.GetComponent<MeshFilter>().mesh.bounds.size.y / 2.0f);

        return true;
    }

    /// <summary>
    /// remove object at specified tile [DESTROYS OBJECT]
    /// </summary>
    /// <returns>false if no object at tile</returns>
    public static bool RemoveObject(Tile tile)
    {
        if (tile == null)
            return false;

        GameObject obj;
        if ((obj = tile.FreeTile()) == null)
            return false;

        Destroy(obj);

        return true;
    }

    public bool ValidTile(Tile tile) => _MouseCollision && tile != null && tile.TileType != TileType.Undefined;

    public bool LeftPressed() => ValidTile(CurrentTile) && Mouse.current.leftButton.wasPressedThisFrame;
    public bool RightPressed() => ValidTile(CurrentTile) && Mouse.current.rightButton.wasPressedThisFrame;

    /// <summary>
    /// updates specified tile type and changes texture on grid accordingly
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="type"></param>
    public void UpdateTile(Tile tile, TileType type)
    {
        tile.UpdateType(type);

        int i = (int)tile.PositionLocal.z + (int)tile.PositionLocal.x * _Grid.Width;

        float tileTexProcRow = 1.0f / _Grid.TexTilesPerRow;
        float tileTexProcCol = 1.0f / _Grid.TexTilesPerCol;

        float proc = (int)tile.TileType / (float)_Grid.TexTilesPerRow;
        float dFix = 0.05f; // dilation

        _Grid.Uvs[i * 4 + 0] = new Vector2(proc + dFix,                  1.0f - dFix); // top-left
        _Grid.Uvs[i * 4 + 1] = new Vector2(proc + tileTexProcRow - dFix, 1.0f - dFix); // top-right
        _Grid.Uvs[i * 4 + 2] = new Vector2(proc + dFix,                  0.0f + dFix); // bottom-left
        _Grid.Uvs[i * 4 + 3] = new Vector2(proc + tileTexProcRow - dFix, 0.0f + dFix); // bottom-right

        _MeshFilter.mesh.uv = _Grid.Uvs;
    }

    public void OnInteract(InteractContext context)
    {
        
    }
}
