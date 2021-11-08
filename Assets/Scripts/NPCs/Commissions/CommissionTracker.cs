using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class CommissionTracker : Singleton<CommissionTracker>
{
    [SerializeField] private TMP_Text _Title;
    [SerializeField] private TMP_Text _Quota;

    private Color _TitleColor;

    private CanvasGroup _CanvasGroup;

    private static Commission _Commission;

    public Commission Commission => _Commission;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = (_Commission != null) ? 1.0f : 0.0f;

        _TitleColor = _Title.color;

        UpdateTracker();
    }

    public void UpdateTracker()
    {
        if (_Commission == null)
            return;

        _Title.text = _Commission.CommissionData.Title;
        _Title.color = _Commission.IsCompleted ? _TitleColor + new Color(-0.2f, 0.5f, -0.2f, 1.0f) : _TitleColor;

        string quota = string.Empty;
        foreach (IObjective objective in _Commission.Objectives)
        {
            quota += objective.Status;
            if (objective != _Commission.Objectives[_Commission.Objectives.Length - 1])
                quota += " | ";
        }

        _Quota.text = quota;
    }

    public void Track(Commission commission)
    {
        _Commission = commission;
        _CanvasGroup.alpha = 1.0f;

        UpdateTracker();
    }

    public void Untrack()
    {
        _CanvasGroup.alpha = 0.0f;
        _Commission = null;
    }
}
