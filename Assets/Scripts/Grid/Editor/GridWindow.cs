using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class GridWindow : OdinEditorWindow
{
    [TableList]
    public List<TileEditor> _Tiles = new List<TileEditor>();

    [MenuItem("Tools/Grid Editor")]
    private static void OpenWindow()
    {
        GridWindow window = GetWindow<GridWindow>();
        window.titleContent = new GUIContent("Grid");
        window.Show();
    }

    protected override void Initialize()
    {
        
    }
}
