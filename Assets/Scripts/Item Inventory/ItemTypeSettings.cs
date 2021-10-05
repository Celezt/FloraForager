using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "ItemTypeSettings", menuName = "Inventory/ItemTypeSettings")]
[System.Serializable]
public class ItemTypeSettings : SerializedScriptableSingleton<ItemTypeSettings>
{
    private const string DESCRIPTION_PATH = "Assets/Data/Items/Item Descriptions";

    public IReadOnlyList<string> Labels => _labelSettings.Labels;
    public IReadOnlyDictionary<string, ItemType> ItemTypeChunk => _itemTypeChunk;
    public IReadOnlyDictionary<string, Sprite> ItemIconChunk => _itemIconChunk;
    public IReadOnlyDictionary<string, string> ItemNameChunk => _itemNameChunk;
    public IReadOnlyDictionary<string, List<string>> ItemLabelChunk => _itemLabelChunk;

    public LabelSettings _labelSettings;
    [OdinSerialize, ReadOnly]
    private Dictionary<string, List<string>> _itemLabelChunk = new Dictionary<string, List<string>>();
    [OdinSerialize, ReadOnly]
    private Dictionary<string, ItemType> _itemTypeChunk = new Dictionary<string, ItemType>();
    [OdinSerialize, ReadOnly]
    private Dictionary<string, Sprite> _itemIconChunk = new Dictionary<string, Sprite>();
    [OdinSerialize, ReadOnly]
    private Dictionary<string, string> _itemNameChunk = new Dictionary<string, string>();

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
        
        _itemTypeChunk.Add(id, itemType);

        if (!_itemLabelChunk.ContainsKey(id))
            _itemLabelChunk.Add(id, itemType.Labels.Select(item => (string)item.Clone()).ToList());
        if (!_itemIconChunk.ContainsKey(id))
            _itemIconChunk.Add(id, itemType.Icon);
        if (!_itemNameChunk.ContainsKey(id))
            _itemNameChunk.Add(id, itemType.Name);

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

        OnRemoveItemTypeCallback.Invoke(id);
        
        _itemLabelChunk.Remove(id);
        _itemTypeChunk.Remove(id);
        _itemIconChunk.Remove(id);
        _itemNameChunk.Remove(id);

        return true;
    }

    public bool RemoveLabel(string name)
    {
        if (!_labelSettings.Labels.Contains(name))
            return false;

        foreach (KeyValuePair<string, List<string>> labels in _itemLabelChunk)
        {
            labels.Value.Remove(name);
        }

        foreach (KeyValuePair<string, ItemType> item in _itemTypeChunk)
        {
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
        
        foreach (KeyValuePair<string, List<string>> labels in _itemLabelChunk)
        {
            labels.Value.ChangeKey(oldLabelName, newLabelName);
        }

        foreach (KeyValuePair<string, ItemType> item in _itemTypeChunk)
        {
            item.Value.Labels.ChangeKey(oldLabelName, newLabelName);
        }

        RemoveLabel(oldLabelName);
    }

    public void AttachLabelForItemType(List<ItemType> itemsTypes, string label)
    {
        AddLabel(label);

        foreach (ItemType item in itemsTypes)
        {
            if (item.Labels.Contains(label))
                continue;

            foreach (KeyValuePair<string, List<string>> labels in _itemLabelChunk)
            {
                if (!labels.Value.Contains(label))
                    labels.Value.Add(label);
            }

            // Add new label at the end of the list.
            SerializedObject so = new SerializedObject(item);
            SerializedProperty labelArray = so.FindProperty(nameof(item.Labels));
            labelArray.InsertArrayElementAtIndex(labelArray.arraySize);
            SerializedProperty toChange = labelArray.GetArrayElementAtIndex(labelArray.arraySize - 1);
            toChange.stringValue = label;
            so.ApplyModifiedProperties();
        }
    }

    public void DetachLabelFromItemType(List<ItemType> itemsTypes, string label)
    {
        foreach (ItemType item in itemsTypes)
        {
            if (!item.Labels.Contains(label))
                continue;

            foreach (KeyValuePair<string, List<string>> labels in _itemLabelChunk)
            {
                labels.Value.Remove(label);
            }

            // Remove label at index.
            SerializedObject so = new SerializedObject(item);
            SerializedProperty labelArray = so.FindProperty(nameof(item.Labels));
            labelArray.DeleteArrayElementAtIndex(item.Labels.IndexOf(label));
            so.ApplyModifiedProperties();
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
            _itemIconChunk.Add(id, newIcon);

        _itemIconChunk[id] = newIcon;
    }

    public void ChangeName(string id, string newName)
    {
        if (string.IsNullOrEmpty(id))
            return;

        if (!_itemNameChunk.ContainsKey(id))
            _itemNameChunk.Add(id, newName);

        string oldName = _itemNameChunk[id];
        _itemNameChunk[id] = newName;
    }
#endif
}
