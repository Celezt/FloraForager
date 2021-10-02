using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "ItemTypeSettings", menuName = "Inventory/ItemTypeSettings")]
[System.Serializable]
public class ItemTypeSettings : ScriptableObject
{
    public static ItemTypeSettings Instance => _instance;

    private static ItemTypeSettings _instance;

    public List<string> Labels => _labels;

    [SerializeField, ReadOnly, ListDrawerSettings(Expanded = true)]
    private List<string> _labels = new List<string>();
    private Dictionary<string, List<string>> _allLabels = new Dictionary<string, List<string>>();

    public bool RemoveLabel(string name) => _labels.Remove(name);

    public int GetIndexOfLabel(string label) => _labels.IndexOf(label);

    public bool AddItemType(ItemType itemType)
    {
        string id = itemType.ID;

        if (_allLabels.ContainsKey(id))
            return false;

        _allLabels.Add(id, itemType.Labels);
        
        return true;
    }

    public bool RemoveItemType(ItemType itemType) => _allLabels.Remove(itemType.ID);

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
        
        foreach (KeyValuePair<string, List<string>> labels in _allLabels)
        {      
            if (labels.Value.Contains(oldLabelName))
            {
                labels.Value.Remove(oldLabelName);
                labels.Value.Add(newLabelName);
            }
        }

        RemoveLabel(oldLabelName);
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
