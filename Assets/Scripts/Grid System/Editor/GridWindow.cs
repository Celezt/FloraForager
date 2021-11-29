using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;

public class GridWindow : OdinEditorWindow
{
    [SerializeField, PropertySpace(10)]
    [HorizontalGroup("Group"), ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 50, IsReadOnly = true), ReadOnly]
    private CellMesh[] _CellsMesh;

    [SerializeField, PropertySpace(10)]
    [HorizontalGroup("Group"), ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 50, IsReadOnly = true)]
    private CellData[] _CellsData;

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

    protected override void OnBeforeSerialize()
    {
        for (int i = 0; i < _CellsMesh.Length; ++i)
        {
            CellMesh mesh = _CellsMesh[i];
            CellData data = _CellsData[i];

            if (mesh == null || data == null)
                continue;

            mesh.Initialize(data);
        }
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

        Refresh();
        _Mesh.Compile(_CellsMesh);
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

        Refresh();
        _Mesh.Decompile();
        Refresh();
    }

    [Button(ButtonSizes.Medium), VerticalGroup, PropertyOrder(-3)]
    private void Refresh()
    {
        _CellsMesh = GameObject.FindObjectsOfType<CellMesh>();
        _CellsData = System.Array.ConvertAll(_CellsMesh, c => c.Data);

        if (_Mesh == null)
        {
            if (FindObjectOfType<GameGrid>() == null)
                Debug.LogError("please add a grid to the scene in order to use the grid tool");
            else
                _Mesh = GameGrid.Instance?.GetComponent<GridMesh>();
        }
        else
            EditorUtility.SetDirty(_Mesh);
    }
}
