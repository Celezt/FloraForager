using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;
using System.IO;
using Newtonsoft.Json;

public class ItemWindow : OdinMenuEditorWindow
{
    private const string TYPE_PATH = "Assets/Data/Items/Item Types";
    private const string DESCRIPTION_PATH = "Assets/Data/Items/Item Descriptions";

    private CustomItem _customItem;
    private ItemTypeSettings _settings;

    [MenuItem("/Tools/Item Editor")]
    private static void OpenWindow()
    {
        ItemWindow window = GetWindow<ItemWindow>();
        window.titleContent = new GUIContent("Items");
        window.Show();        
    }

    protected override void Initialize()
    {
        _settings = ItemTypeSettings.Instance;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(false);
        tree.Config.DrawSearchToolbar = true;

        _customItem = new CustomItem(_settings);
        tree.Add("Create New", _customItem);

        List<ItemType> itemTypes = new List<ItemType>();
        ObjectExtensions.TryGetObjectsOfTypeFromPath<ItemType>(TYPE_PATH, itemTypes);
        foreach (ItemType itemType in itemTypes)
        {
            if (itemType.Labels.Count == 0)
                tree.AddMenuItemAtPath("Default", new OdinMenuItem(tree, itemType.Name, itemType));
            else
            {
                foreach (string label in itemType.Labels)
                    tree.AddMenuItemAtPath(label.FirstCharToUpper(), new OdinMenuItem(tree, itemType.Name, itemType));
            }
        }
        
        return tree;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _customItem?.Destroy();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void OnBeginDrawEditors()
    {
        if (MenuTree == null)
            return;

        OdinMenuTreeSelection selected = MenuTree.Selection;
        ItemType asset = selected.SelectedValue as ItemType;

        void Deserialize()
        {
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

        void Serialize()
        {
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

        void Delete()
        {
            string path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.DeleteAsset($"{DESCRIPTION_PATH}/{asset.ID}_en.json");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void ManageLabels()
        {
            GetWindow<ItemLabelWindow>(true).Create(ItemTypeSettings.Instance);
        }

        void SetLabel(Rect rect)
        {
            Dictionary<string, int> labelCounts = new Dictionary<string, int>();

            foreach (string label in asset.Labels)
            {
                int count;
                labelCounts.TryGetValue(label, out count);
                count++;
                labelCounts[label] = count;
            }

            FocusWindowIfItsOpen<ItemWindow>();
            rect.x = 180;
            rect.y = 20;
            
            PopupWindow.Show(rect, new ItemLabelMaskPopupContent(_settings, new List<ItemType>() { asset }, labelCounts));
        }

        Rect rect = SirenixEditorGUI.BeginHorizontalToolbar();
        {
            if (MenuTree.GetMenuItem("Create New").IsSelected)
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Save"))
                    _customItem.Create();
            }
            else if (selected.Contains(MenuTree.GetMenuItem("Items")))
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                {
                    var guiMode = new GUIContent("Labels");
                    Rect rMode = GUILayoutUtility.GetRect(guiMode, EditorStyles.toolbarDropDown);
                    if (EditorGUI.DropdownButton(rMode, guiMode, FocusType.Passive, EditorStyles.toolbarDropDown))
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("set Labels"), false, () => SetLabel(rMode));
                        menu.AddItem(new GUIContent("Manage Labels"), false, () => ManageLabels());

                        menu.DropDown(rect);
                    }
                }

                {
                    var guiMode = new GUIContent("Convert");
                    Rect rMode = GUILayoutUtility.GetRect(guiMode, EditorStyles.toolbarDropDown);
                    if (EditorGUI.DropdownButton(rMode, guiMode, FocusType.Passive, EditorStyles.toolbarDropDown))
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Serialize"), false, () => Serialize());
                        menu.AddItem(new GUIContent("Deserialize"), false, () => Deserialize());

                        menu.DropDown(rect);
                    }
                }          

                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete"))
                    Delete();

            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }

    public class CustomItem
    {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        [SerializeField]
        private CustomItemType _customItem;

        private ItemType _item;

        private ItemTypeSettings _settings;

        public CustomItem(ItemTypeSettings settings)
        {
            _settings = settings;
            _customItem = ScriptableObject.CreateInstance<CustomItemType>();
            _customItem.Name = "New Item";
            _customItem.ID = "new_item";
        }

        public void Create()
        {
            _item = CreateInstance<ItemType>();
            _item.Icon = _customItem.Icon;
            _item.Name = _customItem.Name;
            _item.ID = _settings.GetUniqueID(_customItem.ID);
            _item.Description = _customItem.Description;
            _item.Behaviour = _customItem.Behaviour;
            _item.Labels = _customItem.Labels;
            _item.Create();

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
            public IItem Behaviour;
            [HideInInspector]
            public List<string> Labels = new List<string>() { };
        }

    }
}
