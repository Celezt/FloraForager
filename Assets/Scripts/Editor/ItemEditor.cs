using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;
using System.IO;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.Linq;

public class ItemEditor : OdinMenuEditorWindow
{
    private const string TYPE_PATH = "Assets/Data/Items/Item Types";
    private const string DESCRIPTION_PATH = "Assets/Data/Items/Item Descriptions";
    private const string DESCRIPTION_LABEL = "item_description";

    private CustomItem _customItem;

    [MenuItem("/Tools/Items")]
    private static void OpenWindow()
    {
        GetWindow<ItemEditor>().Show();
    }


    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();

        _customItem = new CustomItem();
        tree.Add("Create New", _customItem);
        tree.AddAllAssetsAtPath("Items", TYPE_PATH, typeof(ItemType));

        return tree;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _customItem?.Destroy();
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = MenuTree.Selection;

        void Deserialize()
        {
            ItemType asset = selected.SelectedValue as ItemType;

            string[] files = Directory.GetFiles($"{Application.dataPath}/Data/Items/Item Descriptions/", "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

                if (textAsset.name.Substring(0, textAsset.name.LastIndexOf("_")) == asset.ID)
                {
                    ItemDescriptionAsset info = JsonConvert.DeserializeObject<ItemDescriptionAsset>(textAsset.text);
                    asset.Name = info.Name;
                    asset.Description = info.Description;
                }
            }
        }

        void Delete()
        {
            ItemType asset = selected.SelectedValue as ItemType;
            string path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
            File.Delete($"{DESCRIPTION_PATH}/{asset.ID}_en.json");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void Serialize()
        {
            ItemType asset = selected.SelectedValue as ItemType;

            string[] files = Directory.GetFiles($"{Application.dataPath}/Data/Items/Item Descriptions/", "*.json", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

                if (textAsset.name.Substring(0, textAsset.name.LastIndexOf("_")) == asset.ID)
                {
                    ItemDescriptionAsset descriptionAsset = new ItemDescriptionAsset { Description = asset.Description, Name = asset.Name };
                    string serialized = JsonConvert.SerializeObject(descriptionAsset, Formatting.Indented);
                    File.WriteAllText(file, serialized);
                }
            }

            AssetDatabase.Refresh();
        }

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            if (MenuTree.GetMenuItem("Create New").IsSelected)
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Save"))
                    _customItem.Create();

                SirenixEditorGUI.EndHorizontalToolbar();
            }
            else if (MenuTree.GetMenuItem("Items").IsSelected)
            {
                GUILayout.FlexibleSpace();

                SirenixEditorGUI.EndHorizontalToolbar();
            }
            else
            {
                if (SirenixEditorGUI.ToolbarButton("Serialize"))
                    Serialize();

                if (SirenixEditorGUI.ToolbarButton("Deserialize"))
                    Deserialize();

                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete"))
                    Delete();

                SirenixEditorGUI.EndHorizontalToolbar();
            }
        }
    }

    public class CustomItem
    {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        [SerializeField]
        private CustomItemType _customItem;

        private ItemType _item;

        public CustomItem()
        {
            _customItem = ScriptableObject.CreateInstance<CustomItemType>();
            _customItem.Name = "New Item";
            _customItem.ID = "new_item";
        }

        public void Create()
        {
            _item = CreateInstance<ItemType>();
            _item.Icon = _customItem.Icon;
            _item.Name = _customItem.Name;
            _item.ID = _customItem.ID;
            _item.Description = _customItem.Description;
            _item.Reference = _customItem.Reference;
            _item.Labels = _customItem.Labels;

            AssetDatabase.CreateAsset(_item, $"{TYPE_PATH}/{_item.ID}.asset");

            ItemDescriptionAsset descriptionAsset = new ItemDescriptionAsset { Description = _item.Description, Name = _item.Name };
            string serialized = JsonConvert.SerializeObject(descriptionAsset, Formatting.Indented);

            File.WriteAllText($"{DESCRIPTION_PATH}/{_item.ID}_en.json", serialized);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void Destroy()
        {
            DestroyImmediate(_item);
            DestroyImmediate(_customItem);
        }

        public class CustomItemType : SerializedScriptableObject
        {
            [PreviewField(120), HideLabel]
            [HorizontalGroup("Group 1", 120)]
            public Sprite Icon;
            [VerticalGroup("Group 1/Right"), LabelWidth(80)]
            public string Name;
            [VerticalGroup("Group 1/Right"), LabelWidth(80)]
            public string ID;
            [VerticalGroup("Group 1/Right"), TextArea(5, 30)]
            public string Description;
            [Required, OdinSerialize, HideLabel, Indent]
            [ListDrawerSettings(Expanded = true)]
            public IItem Reference;
            public string[] Labels;
        }

    }

}
