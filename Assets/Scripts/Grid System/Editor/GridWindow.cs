using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;

public class GridWindow : OdinEditorWindow
{
    [ReadOnly, PropertySpace(10)]
    public CellMesh[] _Cells;

    private GridMesh _Mesh;

    [MenuItem("Tools/Grid Editor")]
    private static void OpenWindow()
    {
        GridWindow window = GetWindow<GridWindow>();
        window.titleContent = new GUIContent("Grid System");
        window.Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Refresh();
    }

    [PropertySpace(5)]
    [Button(ButtonSizes.Large), HorizontalGroup, PropertyOrder(-2)]
    private void Compile()
    {
        if (_Mesh == null)
        {
            Debug.LogError("could not access GridMesh; try refreshing");
            return;
        }

        _Mesh.Compile(_Cells);

        Refresh();
    }

    [PropertySpace(5)]
    [Button(ButtonSizes.Large), HorizontalGroup, PropertyOrder(-1)]
    private void Decompile()
    {
        if (_Mesh == null)
        {
            Debug.LogError("could not access GridMesh; try refreshing");
            return;
        }

        _Cells = _Mesh.Decompile();

        Refresh();
    }

    [Button(ButtonSizes.Medium), VerticalGroup, PropertyOrder(-3)]
    private void Refresh()
    {
        _Cells = GameObject.FindObjectsOfType<CellMesh>();

        if (_Mesh == null)
        {
            if (FindObjectOfType<Grid>() == null)
                Debug.LogError("please add a grid to the scene in order to use the grid tool");
            else
                _Mesh = Grid.Instance?.GetComponent<GridMesh>();
        }
        else
            EditorUtility.SetDirty(_Mesh);
    }
}
