using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;

public class ItemLabelWindow : EditorWindow
{
    ReorderableList _reorderableLabels;
    private ItemTypeSettings _settings;

    private Vector2 _scrollPosition;
    private int _activeIndex = -1;
    private int _borderSpacing = 7;
    private bool _isEditing = false;
    private string _currentEdit;
    private string _oldName;

    public void Create(ItemTypeSettings settings)
    {
        _settings = settings;

        titleContent = new GUIContent("Item Labels");
        _reorderableLabels = new ReorderableList(_settings._labels, typeof(string), true, false, true, true);
        _reorderableLabels.drawElementCallback += DrawLabelNameCallback;
        _reorderableLabels.onAddDropdownCallback += OnAddLabelCallback;
        _reorderableLabels.onRemoveCallback += OnRemoveLabelCallback;
        _reorderableLabels.onSelectCallback += list =>
        {
            _activeIndex = list.index;
            EndEditMenu();
        };
        _reorderableLabels.headerHeight = 0;    // Hide header completely.

        _activeIndex = -1;
        _isEditing = false;
    }

    private void DrawLabelNameCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        string oldName = _settings.Labels[index];

        if (_isEditing && index == _activeIndex)
        {
            _oldName = oldName;
            GUI.SetNextControlName(_oldName);
            _currentEdit = EditorGUI.TextField(rect, _currentEdit);
            GUI.FocusControl(_oldName);
        }
        else
            EditorGUI.LabelField(rect, oldName);
    }

    private void EndEditMenu()
    {
        _isEditing = false;
        _currentEdit = string.Empty;
        _oldName = string.Empty;
        Repaint();
    }

    private void OnAddLabelCallback(Rect buttonRect, ReorderableList list)
    {
        buttonRect.x = 6;
        buttonRect.y -= 13;

        PopupWindow.Show(buttonRect, new LabelNamePopup(position.width, _reorderableLabels.elementHeight, _settings));
    }

    private void OnRemoveLabelCallback(ReorderableList list)
    {
        _settings.RemoveLabel(_settings.Labels[_reorderableLabels.index]);
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(EditorStyles.label);
        GUILayout.Space(_borderSpacing);
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        _reorderableLabels.DoLayoutList();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        HandleEvent(Event.current);
    }

    private void HandleEvent(Event current)
    {
        if (_activeIndex < 0 || _settings.Labels.Count == 0)
            return;

        if (current.type == EventType.ContextClick)
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Edit"), false, () =>
            {
                _isEditing = true;
                _currentEdit = _settings.Labels[_activeIndex];
                Repaint();
            });
            contextMenu.ShowAsContext();
            Repaint();
        }
        else if (_isEditing && (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter))
        {
            _settings.RenameLabel(_oldName, _currentEdit);
            EndEditMenu();
        }
        else if (current.type == EventType.MouseDown && _isEditing)
            EndEditMenu();
    }

    private class LabelNamePopup : PopupWindowContent
    {
        private ItemTypeSettings _settings;
        private float _windowWidth;
        private float _rowHeight;
        private string _name;
        private bool _needsFocus = true;

        public LabelNamePopup(float windowWidth, float rowHeight, ItemTypeSettings settings)
        {
            _settings = settings;
            _windowWidth = windowWidth;
            _rowHeight = rowHeight;
            _name = _settings.GetUniqueLabel("New Label");
        }

        public override Vector2 GetWindowSize() => new Vector2(_windowWidth - 13.0f, _rowHeight * 2.25f);

        public override void OnGUI(Rect rect)
        {
            GUILayout.Space(5);
            Event e = Event.current;
            bool hitEnter = e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter);
            GUI.SetNextControlName("LabelName");
            EditorGUIUtility.labelWidth = 80;
            _name = EditorGUILayout.TextField("Label Name", _name);

            if (_needsFocus)
            {
                _needsFocus = false;
                EditorGUI.FocusTextInControl("LabelName");
            }

            GUI.enabled = _name.Length != 0;
            if (GUILayout.Button("Save") || hitEnter)
            {
                if (string.IsNullOrEmpty(_name))
                    Debug.LogError("Cannot add empty label to Item label list");
                else if (_name != _settings.GetUniqueLabel(_name))
                    Debug.LogError($"Label name '{_name}' is already in the labels list.");
                else
                {
                    _settings.AddLabel(_name);
                }

                editorWindow.Close();
            }
        }
    }
    
}
