using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemLabelMaskPopupContent : PopupWindowContent
{
    private ItemTypeSettings _settings;
    private List<ItemType> _itemTypes;
    private Dictionary<string, int> _labelCount;

    private int _lastItemCount = -1;
    Vector2 _rect = new Vector2(100, 200);
    Vector2 _scrollPosition;

    public ItemLabelMaskPopupContent(ItemTypeSettings settings, List<ItemType> itemTypes, Dictionary<string, int> labelCount)
    {
        _settings = settings;
        _itemTypes = itemTypes;
        _labelCount = labelCount;
    }

    public override Vector2 GetWindowSize()
    {
        return _rect;
    }

    private void AttachLabelForItem(string label)
    {
        _settings.AttachLabelForItemTypes(_itemTypes, label);

        _labelCount[label] = _itemTypes.Count;
    }

    private void DetachLabelForItem(string label)
    {
        _settings.DetachLabelFromItemTypes(_itemTypes, label);
        _labelCount[label] = 0;
    }

    public override void OnGUI(Rect rect)
    {
        IReadOnlyList<string> labels = _settings.Labels;
        if (_lastItemCount != labels.Count)
        {
            int maxLength = 0;
            string maxString = "";
            for (int i = 0; i < labels.Count; i++)
            {
                int length = labels[i].Length;
                if (length > maxLength)
                {
                    maxLength = length;
                    maxString = labels[i];
                }
            }
            float minWidth;
            float maxWidht;
            GUIContent content = new GUIContent(maxString);

            GUI.skin.toggle.CalcMinMaxWidth(content, out minWidth, out maxWidht);
            var height = GUI.skin.toggle.CalcHeight(content, maxWidht) + 8.5f;
            _rect = new Vector2(Mathf.Clamp(maxWidht + 35, 125, 600), Mathf.Clamp(labels.Count * height + 25, 30, 150));
            _lastItemCount = labels.Count;
        }

        if (_itemTypes.Count == 0)
            return;

        var areaRect = new Rect(rect.xMin + 3, rect.yMin + 3, rect.width - 6, rect.height - 6);
        GUILayout.BeginArea(areaRect);
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, false);
        Vector2 yPositionDrawRange = new Vector2(_scrollPosition.y - 30, _scrollPosition.y + rect.height + 30);

        foreach (string label in labels)
        {
            Rect toggleRect = EditorGUILayout.GetControlRect(GUILayout.Width(areaRect.width - 20));
            if (toggleRect.height > 1)
            {
                // Only draw toggles if they are in view.
                if (toggleRect.y < yPositionDrawRange.x || toggleRect.y > yPositionDrawRange.y)
                    continue;
            }
            else
                continue;

            bool newState;
            int count;
            if (_labelCount == null)
                count = _itemTypes[0].Labels.Contains(label) ? _itemTypes.Count : 0;
            else
                _labelCount.TryGetValue(label, out count);

            bool oldState = count == _itemTypes.Count;
            if (!(count == 0 || count == _itemTypes.Count))
                EditorGUI.showMixedValue = true;
            newState = EditorGUI.ToggleLeft(toggleRect, new GUIContent(label), oldState);
            EditorGUI.showMixedValue = false;

            if (oldState != newState)
            {
                if (newState)
                    AttachLabelForItem(label);
                else
                    DetachLabelForItem(label);
            }

        }

        if (GUILayout.Button("Manage Labels", GUI.skin.button, GUILayout.ExpandWidth(false)))
            EditorWindow.GetWindow<ItemLabelWindow>(true).Create(_settings);

        GUILayout.EndScrollView();
        GUILayout.EndArea();   
    }
}
