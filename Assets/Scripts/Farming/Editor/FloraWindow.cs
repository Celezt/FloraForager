using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class FloraWindow : OdinMenuEditorWindow
{
    private const string TYPE_PATH = "Assets/Data/Flora";

    private CustomFlora _CustomFlora;

    [MenuItem("/Tools/Flora Editor")]
    private static void OpenWindow()
    {
        FloraWindow window = GetWindow<FloraWindow>();
        window.titleContent = new GUIContent("Flora");
        window.Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(false);
        tree.Config.DrawSearchToolbar = true;

        _CustomFlora = new CustomFlora();

        tree.Add("Create New", _CustomFlora);
        tree.AddAllAssetsAtPath("Flora Types", TYPE_PATH, typeof(FloraData));

        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;
        FloraData asset = selected.SelectedValue as FloraData;

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();

            if (MenuTree.GetMenuItem("Create New").IsSelected)
            {
                if (SirenixEditorGUI.ToolbarButton("Create"))
                    _CustomFlora.Create();
            }
            else
            {
                if (SirenixEditorGUI.ToolbarButton("Delete"))
                    Delete();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();

        void Delete()
        {
            string path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _CustomFlora?.Destroy();
    }

    public class CustomFlora
    {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public FloraData FloraData;

        public CustomFlora()
        {
            FloraData = ScriptableObject.CreateInstance<FloraData>();
        }

        public void Create()
        {
            AssetDatabase.CreateAsset(FloraData, $"{TYPE_PATH}/{FloraData.Name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            FloraData = ScriptableObject.CreateInstance<FloraData>();
        }

        public void Destroy()
        {
            DestroyImmediate(FloraData);
        }
    }
}
