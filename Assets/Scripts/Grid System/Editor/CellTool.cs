using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Grid Creation")]
public class CellTool : EditorTool
{
    [SerializeField] 
    private Texture2D _ToolIcon;

    private GUIContent _IconContent;

    private bool _MiddleButtonPressed;

    private Vector3 _CreateAt;

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

        _MiddleButtonPressed = false;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 2)
        {
            _MiddleButtonPressed = true;
        }

        if (_MiddleButtonPressed)
        {
            ShowMenu();
            Event.current.Use();
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView))
            return;

        if (!ToolManager.IsActiveTool(this))
            return;

        Handles.DrawWireDisc(GetCurrentMousePositionInScene(), Vector3.up, 0.5f);



        window.Repaint();
    }

    private Vector3 GetCurrentMousePositionInScene()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        bool placeObject = HandleUtility.PlaceObject(mousePosition, out Vector3 newPosition, out Vector3 normal);
        return placeObject ? newPosition : HandleUtility.GUIPointToWorldRay(mousePosition).GetPoint(10);
    }

    private void ShowMenu()
    {
        string[] names = Enum.GetNames(typeof(CellType));
        if (names.Length <= 0)
            return;

        _CreateAt = GetCurrentMousePositionInScene();

        GenericMenu menu = new GenericMenu();
        for (int i = 1; i < names.Length; ++i)
        {
            menu.AddItem(new GUIContent(names[i]), false, CreateObject, Enum.ToObject(typeof(CellType), i));
        }
        menu.ShowAsContext();
    }

    private void CreateObject(object type)
    {
        GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Grid System/Cell.prefab");
        if (cellPrefab == null)
        {
            Debug.LogError("could not instantiate prefab");
            return;
        }

        GameObject newCellObject = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab);
        newCellObject.GetComponent<CellMesh>().SetType((CellType)type);

        newCellObject.transform.position = Vector3Int.RoundToInt(_CreateAt + Vector3.up);
        newCellObject.transform.SetAsLastSibling();

        Undo.RegisterCreatedObjectUndo(newCellObject, "Place new object");
    }
}
