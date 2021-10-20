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
        if (_Resource == null)
            return;

        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _Resource.transform.position + Vector3.up * _Resource.Bounds.size.y * _HeightOffset);
    }

    public void SetActive(ResourceSource resource, bool active)
    {
        if (active)
        {
            _Resource = resource;
            _Resource.OnQuantityChanged += UpdateText;

            UpdateText();
        }
        else if (_Resource != null)
        {
            _Resource.OnQuantityChanged -= UpdateText;
            _Resource = resource;
        }

        _CanvasGroup.alpha = active ? 1.0f : 0.0f;
    }

    public void UpdateText()
    {
        if (_Resource == null)
            return;

        _AmountLeftText.text = (_Resource.Data.Amount - _Resource.CurrentAmount).ToString() + " " + _Resource.Data.ItemID;
    }
}
