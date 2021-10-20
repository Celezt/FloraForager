using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class CommissionTracker : Singleton<CommissionTracker>
{
    [SerializeField] private Text _Title;
    [SerializeField] private Text _Quota;

    private CanvasGroup _CanvasGroup;

    private static Commission _Commission;

    public Commission Commission => _Commission;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;
    }

    public void UpdateTracker()
    {
        if (_Commission == null)
            return;

        _Title.text = _Commission.IsCompleted ? "<color=green>" + _Commission.Data.Title + "</color>" : _Commission.Data.Title;

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
