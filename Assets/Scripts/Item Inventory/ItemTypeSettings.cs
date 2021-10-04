using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "ItemTypeSettings", menuName = "Inventory/ItemTypeSettings")]
[System.Serializable]
public class ItemTypeSettings : SerializedScriptableObject
{
    private const string DESCRIPTION_PATH = "Assets/Data/Items/Item Descriptions";

    public static ItemTypeSettings Instance => _instance;

    private static ItemTypeSettings _instance;

    public IReadOnlyList<string> Labels => _labels;
    public IReadOnlyDictionary<string, ItemType> ItemTypeChunk => _itemTypeChunk;
    public IReadOnlyDictionary<string, Sprite> ItemIconChunk => _itemIconChunk;

    [SerializeField, ReadOnly, ListDrawerSettings(Expanded = true)]
    public List<string> _labels = new List<string>();
    [SerializeField, ReadOnly]
    private Dictionary<string, HashSet<string>> _itemLabelChunk = new Dictionary<string, HashSet<string>>();
    [SerializeField, ReadOnly]
    private Dictionary<string, ItemType> _itemTypeChunk = new Dictionary<string, ItemType>();
    [SerializeField, ReadOnly]
    private Dictionary<string, Sprite> _itemIconChunk = new Dictionary<string, Sprite>();

   

    public bool RemoveLabel(string name) => _labels.Remove(name);

    public int GetIndexOfLabel(string label) => _labels.IndexOf(label);

    public bool AddItemType(ItemType itemType)
    {
        string id = itemType.ID;

        if (_itemTypeChunk.ContainsKey(id))
            return false;
        
        _itemLabelChunk.Add(id, itemType.Labels);
        _itemTypeChunk.Add(id, itemType);
        _itemIconChunk.Add(id, itemType.Icon);

        return true;
    }

    public bool RemoveItemType(ItemType itemType) => RemoveItemType(itemType.ID);
    public bool RemoveItemType(string id) => _itemLabelChunk.Remove(id) && _itemTypeChunk.Remove(id) && _itemIconChunk.Remove(id);

    public bool AddLabel(string name)
    {
        if (_labels.Contains(name))
            return false;

        _labels.Add(name);
        return true;
    }

    public bool AddLabel(string name, int index)
    {
        if (_labels.Contains(name))
            return false;

        _labels.Insert(index, name);
        return true;
    }

    public string GetUniqueLabel(string name)
    {
        var newName = name;
        int counter = 1;
        while (counter < 100)
        {
            if (!_labels.Contains(newName))
                return newName;
            newName = name + counter;
            counter++;
        }
        return string.Empty;
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
            if (labels.Value.Contains(oldLabelName))
            {
                labels.Value.Remove(oldLabelName);
                labels.Value.Add(newLabelName);
            }
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

    public void RenameID(string oldID, string newID)
    {
        _itemTypeChunk.ChangeKey(oldID, newID);
        _itemIconChunk.ChangeKey(oldID, newID);
        _itemLabelChunk.ChangeKey(oldID, newID);

        AssetDatabase.RenameAsset($"{DESCRIPTION_PATH}/{oldID}_en.json", $"{newID}_en.json");
        AssetDatabase.Refresh();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private void OnEnable()
    {
        if (_instance == null)
            _instance = this;
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}
