using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Grid Creation")]
public class CellTool : EditorTool
{
    [SerializeField] 
    private Texture2D _ToolIcon;

    private GUIContent _IconContent;

    private bool _MiddleButtonPressed = false;

    private bool _RightButtonPressed = false;
    private bool _RightButtonHeld = false;
    private bool _RightButtonReleased = false;

    private bool _QPressed = false;
    private bool _EPressed = false;

    private Vector2 _Scroll;

    private Vector3 _MousePosition;
    private Vector3 _CreateAt;
    private Vector3 _Start;

    public override GUIContent toolbarIcon 
    { 
        get 
        { 
            if (_IconContent == null)
            {
                _IconContent = new GUIContent()
                {
                    image = _ToolIcon,
                    text = "Grid Creation",
                    tooltip = "Grid Creation"
                };
            }
            return _IconContent; 
        } 
    }

    public override void OnActivated()
    {
        SceneView.beforeSceneGui += BeforeSceneGUI;
    }

    public override void OnWillBeDeactivated()
    {
        SceneView.beforeSceneGui -= BeforeSceneGUI;
    }

    public void BeforeSceneGUI(SceneView sceneView)
    {
        if (!ToolManager.IsActiveTool(this))
            return;

        _QPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q;
        _EPressed = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E;

        _Scroll = Event.current.type == EventType.ScrollWheel ? Event.current.delta : Vector2.zero;

        if (_QPressed)
            Event.current.Use();
        if (_EPressed)
            Event.current.Use();
        if (_Scroll != Vector2.zero)
            Event.current.Use();

        if (Event.current.alt || Event.current.control)
            return;

        _MiddleButtonPressed = Event.current.type == EventType.MouseDown && Event.current.button == 2;
        _RightButtonPressed = Event.current.type == EventType.MouseDown && Event.current.button == 1;

        _RightButtonReleased = Event.current.type == EventType.MouseUp && Event.current.button == 1;

        if (_RightButtonPressed)
            _RightButtonHeld = true;
        if (_RightButtonReleased)
            _RightButtonHeld = false;

        if (_RightButtonPressed)
            Event.current.Use();
        if (_MiddleButtonPressed)
            Event.current.Use();
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView))
            return;

        if (!ToolManager.IsActiveTool(this))
            return;

        _MousePosition = GetCurrentMousePositionInScene();

        Handles.DrawWireDisc(_MousePosition, Vector3.up, 0.4f);

        ChangeTypeTool();
        CreateTypeTool();
        FillTool();

        window.Repaint();
    }

    private void ChangeTypeTool()
    {
        int changeType = 0;

        if (_QPressed || _Scroll.y > 0f)
            changeType = -1;
        if (_EPressed || _Scroll.y < 0f)
            changeType = 1;

        if (changeType == 0)
            return;

        Filter();

        int range = Enum.GetValues(typeof(CellType)).Length;

        foreach (GameObject active in Selection.objects)
        {
            if (!active.TryGetComponent(out CellMesh cell))
                continue;

            int newType = (int)cell.Data.Type + changeType;

            if (newType <= 0)
                newType = range - 1;
            if (newType >= range)
                newType = 1;

            cell.SetType((CellType)newType);
        }
    }

    private void Filter()
    {
        List<UnityEngine.Object> selectedCells = new List<UnityEngine.Object>();
        foreach (GameObject active in Selection.objects)
        {
            if (active.GetComponent<CellMesh>() != null)
                selectedCells.Add(active);
        }
        Selection.objects = selectedCells.ToArray();
    }

    private void CreateTypeTool()
    {
        if (_MiddleButtonPressed)
        {
            ShowMenu();
        }
    }

    private void FillTool()
    {
        if (_RightButtonPressed)
        {
            _Start = Vector3Int.RoundToInt(_MousePosition);
        }
        if (_RightButtonHeld)
        {
            Handles.DrawLine(_Start, Vector3Int.RoundToInt(_MousePosition), 3.0f);
        }
        if (_RightButtonReleased)
        {
            CreateCells();
        }
    }

    private void CreateCells()
    {
        Vector3 direction = _MousePosition - _Start;
        Vector2Int boundary = Vector2Int.RoundToInt(new Vector3(direction.x + Mathf.Sign(direction.x), direction.z + Mathf.Sign(direction.z)));

        Vector3 createAt = _Start;

        int boundWidth = Mathf.Abs(boundary.x);
        int boundHeight = Mathf.Abs(boundary.y);

        GameObject[] cells = new GameObject[boundWidth * boundHeight];
        for (int x = 0; x < boundWidth; ++x)
        {
            for (int y = 0; y < boundHeight; ++y)
            {
                _CreateAt = createAt + new Vector3(x * Mathf.Sign(boundary.x), 0, y * Mathf.Sign(boundary.y));
                cells[x + y * boundWidth] = CreateCell(CellType.Empty);
            }
        }

        Selection.objects = cells;
    }

    private void ShowMenu()
    {
        string[] names = Enum.GetNames(typeof(CellType));
        if (names.Length <= 0)
            return;

        _CreateAt = _MousePosition;

        GenericMenu menu = new GenericMenu();
        for (int i = 1; i < names.Length; ++i)
        {
            menu.AddItem(new GUIContent(names[i]), false, (object type) => 
            {
                CreateCell((CellType)type);
            }, 
            Enum.ToObject(typeof(CellType), i));
        }
        menu.ShowAsContext();
    }

    private GameObject CreateCell(CellType type)
    {
        GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Grid System/Cell.prefab");
        if (cellPrefab == null)
        {
            Debug.LogError("could not instantiate prefab");
            return null;
        }

        GameObject cellObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab);
        cellObject.GetComponent<CellMesh>().SetType(type);

        cellObject.transform.position = Vector3Int.RoundToInt(_CreateAt + Vector3.up);
        cellObject.transform.SetAsLastSibling();

        Undo.RegisterCreatedObjectUndo(cellObject, "Place new object");

        return cellObject;
    }

    private Vector3 GetCurrentMousePositionInScene()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        bool placeObject = HandleUtility.PlaceObject(mousePosition, out Vector3 newPosition, out Vector3 normal);
        return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
    }
}
