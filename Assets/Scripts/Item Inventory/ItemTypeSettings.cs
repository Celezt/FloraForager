using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ItemTypeSettings", menuName = "Inventory System/Item Type Settings")]
[System.Serializable]
public class ItemTypeSettings : SerializedScriptableSingleton<ItemTypeSettings>
{
    private const string DESCRIPTION_PATH = "Assets/Data/Items/Item Descriptions";

    public IReadOnlyList<string> Labels => _labelSettings.Labels;
    public IReadOnlyDictionary<string, ItemType> ItemTypeChunk => _itemTypeChunk;
    public IReadOnlyDictionary<string, Sprite> ItemIconChunk => _itemIconChunk;
    public IReadOnlyDictionary<string, string> ItemNameChunk => _itemNameChunk;
    public IReadOnlyDictionary<string, int> ItemStackChunk => _itemStackChunk;
    public IReadOnlyDictionary<string, List<string>> ItemLabelChunk => _itemLabelChunk;

    public LabelSettings LabelSettings => _labelSettings;
    public Sprite DefaultIcon => _defaultIcon;
    public ItemBehaviour ItemObject => _itemObject; 

    [SerializeField]
    private LabelSettings _labelSettings;
    [SerializeField, Tooltip("Default icon used when no other icon is present.")]
    private Sprite _defaultIcon;
    [SerializeField]
    private ItemBehaviour _itemObject;
    [OdinSerialize]
    private Dictionary<string, ItemType> _itemTypeChunk = new Dictionary<string, ItemType>();
    [OdinSerialize]
    private Dictionary<string, Sprite> _itemIconChunk = new Dictionary<string, Sprite>();
    [OdinSerialize]
    private Dictionary<string, string> _itemNameChunk = new Dictionary<string, string>();
    [OdinSerialize]
    private Dictionary<string, int> _itemStackChunk = new Dictionary<string, int>();
    [OdinSerialize]
    private Dictionary<string, List<string>> _itemLabelChunk = new Dictionary<string, List<string>>();

    public int GetIndexOfLabel(string label) => _labelSettings.Labels.IndexOf(label);

    /// <summary>
    /// Get unique <see cref="ItemType"/> id by adding a number at the end.
    /// </summary>
    /// <param name="id">To uniquify.</param>
    /// <returns>Empty <see cref="string"/> if not successful.</returns>
    public string GetUniqueID(string id)
    {
        string newId = id;
        int counter = 1;
        while (counter < 100)
        {
            if (!_itemTypeChunk.ContainsKey(newId))
                return newId;
            newId = id + "_" + counter;
            counter++;
        }
        return string.Empty;
    }

    /// <summary>
    /// Get unique label name by adding a number at the end.
    /// </summary>
    /// <param name="name">To uniquify.</param>
    /// <returns>Empty <see cref="string"/> if not successful.</returns>
    public string GetUniqueLabel(string name)
    {
        var newName = name;
        int counter = 1;
        while (counter < 100)
        {
            if (!_labelSettings.Labels.Contains(newName))
                return newName;
            newName = name + counter;
            counter++;
        }
        return string.Empty;
    }

#if UNITY_EDITOR
    public event Action<ItemType> OnAddItemTypeCallback = delegate { };
    public event Action<string> OnRemoveItemTypeCallback = delegate { };
    public event Action<string> OnAddLabelCallback = delegate { };
    public event Action<string> OnRemoveLabelCallback = delegate { };

    [Button]
    public void Regenerate()
    {
       // DEPRECATED: Enums no longer used because of flag limit.
       // EnumGenerator.Generate("Items", "Assets/Data/Generated", _itemTypeChunk.Keys.ToList());

        _itemLabelChunk.Clear();
        _itemIconChunk.Clear();
        _itemStackChunk.Clear();
        _itemNameChunk.Clear();

        foreach (KeyValuePair<string, ItemType> item in _itemTypeChunk)
            UpdateItemType(item.Value);
    }

    public void UpdateItemType(ItemType itemType)
    {
        string id = itemType.ID;

        if (!_itemTypeChunk.ContainsKey(id))
            return;

        if (!_itemLabelChunk.ContainsKey(id))
            _itemLabelChunk.Add(id, itemType.Labels.Select(item => (string)item.Clone()).ToList());
        if (!_itemIconChunk.ContainsKey(id))
            _itemIconChunk.Add(id, itemType.Icon == null ? _defaultIcon : itemType.Icon);
        if (!_itemStackChunk.ContainsKey(id))
            _itemStackChunk.Add(id, itemType.ItemStack);
        if (!_itemNameChunk.ContainsKey(id))
            _itemNameChunk.Add(id, itemType.Name);
    }

    /// <summary>
    /// Add new <see cref="ItemType"/> to all chunks.
    /// </summary>
    /// <param name="itemType">To add.</param>
    /// <returns>If not already existing.</returns>
    public bool AddItemType(ItemType itemType)
    {
        string id = itemType.ID;

        if (_itemTypeChunk.ContainsKey(id))
            return false;

        EditorUtility.SetDirty(this);

        _itemTypeChunk.Add(id, itemType);

        UpdateItemType(itemType);

        OnAddItemTypeCallback.Invoke(itemType);

        return true;
    }

    /// <summary>
    /// Remove <see cref="ItemType"/> to all chunks.
    /// </summary>
    /// <param name="itemType">To remove.</param>
    /// <returns>If exist.</returns>
    public bool RemoveItemType(ItemType itemType) => RemoveItemType(itemType.ID);

    /// <summary>
    /// Remove <see cref="ItemType"/> to all chunks.
    /// </summary>
    /// <param name="id">To remove.</param>
    /// <returns>If exist.</returns>
    public bool RemoveItemType(string id)
    {
        if (!_itemTypeChunk.ContainsKey(id))
            return false;

        EditorUtility.SetDirty(this);

        OnRemoveItemTypeCallback.Invoke(id);
        
        _itemLabelChunk.Remove(id);
        _itemTypeChunk.Remove(id);
        _itemIconChunk.Remove(id);
        _itemNameChunk.Remove(id);
        _itemStackChunk.Remove(id);

        return true;
    }

    public bool RemoveLabel(string name)
    {
        if (!_labelSettings.Labels.Contains(name))
            return false;

        EditorUtility.SetDirty(this);

        foreach (KeyValuePair<string, List<string>> labels in _itemLabelChunk)
            labels.Value.Remove(name);

        foreach (KeyValuePair<string, ItemType> item in _itemTypeChunk)
        {
            EditorUtility.SetDirty(this);
            item.Value.Labels.Remove(name);
        }

        OnRemoveLabelCallback.Invoke(name);

        // Remove label at index.
        SerializedObject so = new SerializedObject(_labelSettings);
        SerializedProperty labelArray = so.FindProperty(nameof(_labelSettings.Labels));
        labelArray.DeleteArrayElementAtIndex(_labelSettings.Labels.IndexOf(name));
        so.ApplyModifiedProperties();

        return true;
    }

    public bool AddLabel(string name)
    {
        if (_labelSettings.Labels.Contains(name))
            return false;

        // Add new label at the end of the list.
        SerializedObject so = new SerializedObject(_labelSettings); 
        SerializedProperty labelArray = so.FindProperty(nameof(_labelSettings.Labels));
        labelArray.InsertArrayElementAtIndex(labelArray.arraySize);
        SerializedProperty toChange = labelArray.GetArrayElementAtIndex(labelArray.arraySize - 1);
        toChange.stringValue = name;
        so.ApplyModifiedProperties();

        OnAddLabelCallback.Invoke(name);

        return true;
    }

    public bool AddLabel(string name, int index)
    {
        if (_labelSettings.Labels.Contains(name))
            return false;

        // Add new label at index.
        SerializedObject so = new SerializedObject(_labelSettings);
        SerializedProperty labelArray = so.FindProperty(nameof(_labelSettings.Labels));
        SerializedProperty toChange = labelArray.GetArrayElementAtIndex(index);
        toChange.stringValue = name;
        so.ApplyModifiedProperties();

        OnAddLabelCallback.Invoke(name);

        return true;
    }

    public void RenameLabel(string oldLabelName, string newLabelName)
    {
        int index = GetIndexOfLabel(oldLabelName);
        if (index < 0)
            return;

        if (!AddLabel(newLabelName, index))
            return;

        {
            // Serialize and Rename label inside of _itemLabelChunk.
            SerializedObject so = new SerializedObject(this);
            SerializedProperty serializationData = so.FindProperty("serializationData");
            SerializedProperty serializationNodes = serializationData.FindPropertyRelative("SerializationNodes");
            int length = serializationNodes.arraySize;
            for (int i = 0; i < length; i++)
            {
                if (serializationNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue == nameof(_itemLabelChunk))
                {
                    int j = i + 4;
                    while(j < length)
                    {
                        SerializedProperty element = serializationNodes.GetArrayElementAtIndex(j);
                        SerializedProperty dictionaryData = element.FindPropertyRelative("Data");
                        SerializedProperty dictionaryName = element.FindPropertyRelative("Name");

                        // If next serialized data outside of _itemLabelChunk was found.
                        if (!string.IsNullOrEmpty(dictionaryName.stringValue))
                            if (dictionaryName.stringValue[0] != '$')
                                break;

                        // If id exist inside of _itemLabelChunk.
                        if (_itemLabelChunk.ContainsKey(dictionaryData.stringValue))
                        {
                            int listLength = int.Parse(serializationNodes.GetArrayElementAtIndex(j + 2).FindPropertyRelative("Data").stringValue);
                            for (int k = j + 3; k <= j + 2 + listLength; k++)
                            {
                                SerializedProperty listData = serializationNodes.GetArrayElementAtIndex(k).FindPropertyRelative("Data");
                                if (listData.stringValue == oldLabelName)
                                {
                                    listData.stringValue = newLabelName;
                                    break;
                                }
                            }
                            j += 3 + listLength;
                        }
                        j++;
                    }
                    break;
                }
            }

            so.ApplyModifiedProperties();
        }

        foreach (KeyValuePair<string, ItemType> item in _itemTypeChunk)
        {
            item.Value.Labels.ChangeKey(oldLabelName, newLabelName);
        }

        RemoveLabel(oldLabelName);
    }

    struct a
    {
        public string Name;
        public int Entry;
        public string Data;
    }

    public void AttachLabelForItemTypes(List<ItemType> itemsTypes, string label)
    {
        AddLabel(label);

        foreach (ItemType item in itemsTypes)
        {
            if (item.Labels.Contains(label))
                continue;

            string id = item.ID;

            if (!_itemLabelChunk[id].Contains(label))
                _itemLabelChunk[id].Add(label);

            {
                // Serialize and add label inside of _itemLabelChunk.
                SerializedObject so = new SerializedObject(this);
                SerializedProperty serializationData = so.FindProperty("serializationData");
                SerializedProperty serializationNodes = serializationData.FindPropertyRelative("SerializationNodes");
                int length = serializationNodes.arraySize;
                for (int i = 0; i < length; i++)
                {
                    if (serializationNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue == nameof(_itemLabelChunk))
                    {
                        for (int j = i + 4; j < length; j++)
                        {
                            if (serializationNodes.GetArrayElementAtIndex(j).FindPropertyRelative("Data").stringValue == id)
                            {
                                int listLength = int.Parse(serializationNodes.GetArrayElementAtIndex(j + 2).FindPropertyRelative("Data").stringValue);
                                serializationNodes.InsertArrayElementAtIndex(j + 2 + listLength);

                                SerializedProperty element = serializationNodes.GetArrayElementAtIndex(j + 2 + listLength);
                                element.FindPropertyRelative("Name").stringValue = "";
                                element.FindPropertyRelative("Entry").intValue = 1;
                                element.FindPropertyRelative("Data").stringValue = label;
                                break;
                            }
                        }
                        break;
                    }
                }

                so.ApplyModifiedProperties();
            }

            {
                // Add new label at the end of the list.
                SerializedObject so = new SerializedObject(item);
                SerializedProperty labelArray = so.FindProperty(nameof(item.Labels));
                labelArray.InsertArrayElementAtIndex(labelArray.arraySize);
                SerializedProperty toChange = labelArray.GetArrayElementAtIndex(labelArray.arraySize - 1);
                toChange.stringValue = label;
                so.ApplyModifiedProperties();
            }
        }
    }

    public void DetachLabelFromItemTypes(List<ItemType> itemsTypes, string label)
    {
        foreach (ItemType item in itemsTypes)
        {
            if (!item.Labels.Contains(label))
                continue;

            string id = item.ID;

            if (_itemLabelChunk[id].Contains(label))
                _itemLabelChunk[id].Remove(label);

            {
                // Serialize and remove label inside of _itemLabelChunk.
                SerializedObject so = new SerializedObject(this);
                SerializedProperty serializationData = so.FindProperty("serializationData");
                SerializedProperty serializationNodes = serializationData.FindPropertyRelative("SerializationNodes");
                int length = serializationNodes.arraySize;
                for (int i = 0; i < length; i++)
                {
                    if (serializationNodes.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue == nameof(_itemLabelChunk))
                    {
                        for (int j = i + 4; j < length; j++)
                        {
                            if (serializationNodes.GetArrayElementAtIndex(j).FindPropertyRelative("Data").stringValue == id)
                            {
                                int listLength = int.Parse(serializationNodes.GetArrayElementAtIndex(j + 2).FindPropertyRelative("Data").stringValue);

                                for (int k = j + 3; k < j + 2 + listLength; k++)
                                {
                                    if (serializationNodes.GetArrayElementAtIndex(k).FindPropertyRelative("Data").stringValue == label)
                                    {
                                        serializationNodes.DeleteArrayElementAtIndex(k);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }

                so.ApplyModifiedProperties();
            }

            {
                // Remove label at index.
                SerializedObject so = new SerializedObject(item);
                SerializedProperty labelArray = so.FindProperty(nameof(item.Labels));
                labelArray.DeleteArrayElementAtIndex(item.Labels.IndexOf(label));
                so.ApplyModifiedProperties();
            }
        }
    }

    public void RenameID(string oldID, string newID)
    {
        _itemTypeChunk.ChangeKey(oldID, newID);
        _itemIconChunk.ChangeKey(oldID, newID);
        _itemNameChunk.ChangeKey(oldID, newID);
        _itemLabelChunk.ChangeKey(oldID, newID);

        EditorUtility.SetDirty(this);
        AssetDatabase.RenameAsset($"{DESCRIPTION_PATH}/{oldID}_en.json", $"{newID}_en.json");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void ChangeIcon(string id, Sprite newIcon)
    {
        if (string.IsNullOrEmpty(id))
            return;


        if (!_itemIconChunk.ContainsKey(id))
            _itemIconChunk.Add(id, newIcon == null ? _defaultIcon : newIcon);
        else
            _itemIconChunk[id] = newIcon == null ? _defaultIcon : newIcon;

        EditorUtility.SetDirty(this);
    }

    public void ChangeName(string id, string newName)
    {
        if (string.IsNullOrEmpty(id))
            return;


        if (!_itemNameChunk.ContainsKey(id))
            _itemNameChunk.Add(id, newName);
        else
            _itemNameChunk[id] = newName;

        EditorUtility.SetDirty(this);
    }

    public void ChangeItemStack(string id, int newStack)
    {
        if (string.IsNullOrEmpty(id))
            return;


        if (!_itemStackChunk.ContainsKey(id))
            _itemStackChunk.Add(id, newStack);
        else
            _itemStackChunk[id] = newStack;

        EditorUtility.SetDirty(this);
    }
#endif
}
