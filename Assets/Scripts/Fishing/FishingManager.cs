using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private RectTransform _fishArea;
    [SerializeField] private RectTransform _focusArea;
    [SerializeField, Min(0)] private int _fishHeight = 500;

    private void Start()
    {
        SetHeight();
    }

    private void SetHeight()
    {
        _fishArea.sizeDelta = new Vector2(_fishArea.rect.width, _fishHeight);
    }

#if UNITY_EDITOR
    private int _oldHeight;

    private void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    private void _OnValidate()
    {
        if (this == null)
            return;

        if (_oldHeight != _fishHeight)
            SetHeight();

        _oldHeight = _fishHeight;
    }
#endif
}
