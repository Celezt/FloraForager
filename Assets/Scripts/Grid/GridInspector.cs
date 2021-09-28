using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Grid))]
public class GridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Grid grid = (Grid)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate"))
        {
            grid.BuildMesh();
        }
    }
}
#endif