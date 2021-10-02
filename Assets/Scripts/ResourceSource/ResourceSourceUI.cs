using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class ResourceSourceUI : Singleton<ResourceSourceUI>
{
    [SerializeField] private Text _AmountLeftText;
    [SerializeField] private float _HeightOffset = 2.0f;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private ResourceSource _Resource;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = transform.parent.GetComponent<Canvas>();
        _CanvasRect = transform.parent.GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    public void SetActive(ResourceSource resource, bool value)
    {
        _Resource = resource;        

        UpdateText();
        _CanvasGroup.alpha = value ? 1.0f : 0.0f;
    }

    private void UpdatePosition()
    {
        if (_Resource == null)
            return;

        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _Resource.transform.position + Vector3.up * _Resource.Bounds.size.y * _HeightOffset);
    }

    public void UpdateText()
    {
        if (_Resource == null)
            return;

        _AmountLeftText.text = (_Resource.Data.Amount - _Resource.CurrentAmount).ToString() + " " + _Resource.Data.ItemID;
    }
}
