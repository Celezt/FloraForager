using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class WateringCanUI : Singleton<WateringCanUI>
{
    [SerializeField]
    private Image _water;
    [SerializeField]
    private TMP_Text _uses;

    private CanvasGroup _canvasGroup;

    private WateringCanItem _wateringCan;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _canvasGroup.alpha = 0.0f;
    }

    public void LateUpdate()
    {
        if (_wateringCan == null)
            return;

        _uses.text = $"{_wateringCan.UsesLeft}/{_wateringCan.MaxUses}";
        _water.fillAmount = _wateringCan.UsesLeft / (float)_wateringCan.MaxUses;
    }

    public void Show(WateringCanItem wateringCan)
    {
        _wateringCan = wateringCan;
        _canvasGroup.alpha = 1.0f;
    }

    public void Hide()
    {
        _wateringCan = null;
        _canvasGroup.alpha = 0.0f;
    }
}
