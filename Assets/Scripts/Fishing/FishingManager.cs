using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private RectTransform _mainPanel;
    [SerializeField, Min(0)] private int _width = 500;

    private void Start()
    {
        SetWidth();
    }

    private void SetWidth()
    {
        Rect rect = _mainPanel.rect;
        rect.width = _width;
        _mainPanel.sizeDelta = new Vector2(_width, _mainPanel.rect.height);
    }

#if UNITY_EDITOR
    private int _oldWidth;

    private void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    private void _OnValidate()
    {
        if (this == null)
            return;

        if (_oldWidth != _width)
            SetWidth();

        _oldWidth = _width;
    }
#endif
}
