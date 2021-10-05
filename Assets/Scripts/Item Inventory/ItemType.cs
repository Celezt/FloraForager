using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using UnityEditor;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "New ItemType", menuName = "Inventory/ItemType")]
[System.Serializable]
public class ItemType : SerializedScriptableObject
{
    public string ID { get; set; }

    [PreviewField(120), HideLabel]
    [HorizontalGroup("Group 1", 120)]
    public Sprite Icon;
    [VerticalGroup("Group 1/Right"), LabelWidth(80)]
    public string Name;
    [SerializeField, VerticalGroup("Group 1/Right"), LabelWidth(80), ReadOnly]
    private string _id;
    [VerticalGroup("Group 1/Right"), TextArea(5, 30)]
    public string Description;
    [Required, OdinSerialize, HideLabel]
    [ListDrawerSettings(Expanded = true)]
    public IItem Behaviour;

    public List<string> Labels = new List<string>();

    private bool _initialized;

    private void Awake()
    {
#if UNITY_EDITOR
        Create();

        if (_initialized)
            Rename();
#endif
    }

#if UNITY_EDITOR
    public void Create()
    {
        if (_initialized)
            return;

        if (string.IsNullOrEmpty(ID))
        {
            ID = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
        }

        if (!string.IsNullOrEmpty(ID))
        {
            ItemTypeSettings.Instance.AddItemType(this);
            _initialized = true;
        }

        _id = ID;
    }

    public void Rename()
    {
        string oldID = ID;
        ID = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
        _id = ID;

        if (oldID != ID)
        {
            ItemTypeSettings.Instance.RenameID(oldID, ID);
        }
    }
    private void OnValidate()
    {
        ItemTypeSettings settings = ItemTypeSettings.Instance;

        if (settings == null)
            return;

        if (Icon != null)
            settings.ChangeIcon(ID, Icon);

        if (!string.IsNullOrEmpty(Name))
            settings.ChangeName(ID, Name);
    }

    public class DescriptorDeleteDetector : UnityEditor.AssetModificationProcessor
    {
        static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
        {
            if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(ItemType))
            {
                ItemTypeSettings.Instance.RemoveItemType(Path.GetFileNameWithoutExtension(path));
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
#endif
}
