using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using System.IO;
using System;

[CreateAssetMenu(fileName = "ItemTypeSettings", menuName = "Inventory/ItemTypeSettings")]
[System.Serializable]
public class ItemTypeSettings : SerializedScriptableSingleton<ItemTypeSettings>
{
    private const string DESCRIPTION_PATH = "Assets/Data/Items/Item Descriptions";

    public event Action<ItemType> OnAddItemTypeCallback = delegate { };
    public event Action<string> OnRemoveItemTypeCallback = delegate { };
    public event Action<string> OnAddLabelCallback = delegate { };
    public event Action<string> OnRemoveLabelCallback = delegate { };

    public IReadOnlyList<string> Labels => _labelData.Label;
    public IReadOnlyDictionary<string, ItemType> ItemTypeChunk => _itemTypeChunk;
    public IReadOnlyDictionary<string, Sprite> ItemIconChunk => _itemIconChunk;
    public IReadOnlyDictionary<string, string> ItemNameChunk => _itemNameChunk;
    public IReadOnlyDictionary<string, HashSet<string>> ItemLabelChunk => _itemLabelChunk;

    //[SerializeField, ReadOnly, ListDrawerSettings(Expanded = true)]
    //public List<string> _labels = new List<string>();
    public LabelData _labelData;
    [OdinSerialize, ReadOnly]
    private Dictionary<string, HashSet<string>> _itemLabelChunk = new Dictionary<string, HashSet<string>>();
    [SerializeField, ReadOnly]
    private Dictionary<string, ItemType> _itemTypeChunk = new Dictionary<string, ItemType>();
    [SerializeField, ReadOnly]
    private Dictionary<string, Sprite> _itemIconChunk = new Dictionary<string, Sprite>();
    [SerializeField, ReadOnly]
    private Dictionary<string, string> _itemNameChunk = new Dictionary<string, string>();

    public int GetIndexOfLabel(string label) => _labelData.Label.IndexOf(label);

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
            if (!_labelData.Label.Contains(newName))
                return newName;
            newName = name + counter;
            counter++;
        }
        return string.Empty;
    }

#if UNITY_EDITOR
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
        
        _itemLabelChunk.Add(id, itemType.Labels);
        _itemTypeChunk.Add(id, itemType);
        _itemIconChunk.Add(id, itemType.Icon);
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
        if (!_labelData.Label.Contains(name))
            return false;

        foreach (KeyValuePair<string, HashSet<string>> labels in _itemLabelChunk)
        {
            labels.Value.Remove(name);
        }

        foreach (KeyValuePair<string, ItemType> item in _itemTypeChunk)
        {
            item.Value.Labels.Remove(name);
        }

        OnRemoveLabelCallback.Invoke(name);

        // Remove label at index.
        SerializedObject so = new SerializedObject(_labelData);
        SerializedProperty labelArray = so.FindProperty(nameof(_labelData.Label));
        labelArray.DeleteArrayElementAtIndex(_labelData.Label.IndexOf(name));
        so.ApplyModifiedProperties();

        return true;
    }

    public bool AddLabel(string name)
    {
        if (_labelData.Label.Contains(name))
            return false;

        // Add new label at the end of the list.
        SerializedObject so = new SerializedObject(_labelData); 
        SerializedProperty labelArray = so.FindProperty(nameof(_labelData.Label));
        labelArray.InsertArrayElementAtIndex(labelArray.arraySize);
        SerializedProperty toChange = labelArray.GetArrayElementAtIndex(labelArray.arraySize - 1);
        toChange.stringValue = name;
        so.ApplyModifiedProperties();

        OnAddLabelCallback.Invoke(name);

        return true;
    }

    public bool AddLabel(string name, int index)
    {
        if (_labelData.Label.Contains(name))
            return false;

        // Add new label at index.
        SerializedObject so = new SerializedObject(_labelData);
        SerializedProperty labelArray = so.FindProperty(nameof(_labelData.Label));
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
        
        foreach (KeyValuePair<string, HashSet<string>> labels in _itemLabelChunk)
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
            item.Labels.Add(label);
        }
    }

    public void DetachLabelFromItemType(List<ItemType> itemsTypes, string label)
    {
        foreach (ItemType item in itemsTypes)
        {
            item.Labels.Remove(label);
        }
    }

    public void RenameID(string oldID, string newID)
    {
        _itemTypeChunk.ChangeKey(oldID, newID);
        _itemIconChunk.ChangeKey(oldID, newID);
        _itemNameChunk.ChangeKey(oldID, newID);
        _itemLabelChunk.ChangeKey(oldID, newID);

        AssetDatabase.RenameAsset($"{DESCRIPTION_PATH}/{oldID}_en.json", $"{newID}_en.json");
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

        _itemNameChunk[id] = newName;
    }
#endif
}
