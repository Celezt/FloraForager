using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MyBox;

/// <summary>
/// Keeps track of and manages all of the accepted commissions
/// </summary>
public class CommissionLog : Singleton<CommissionLog>
{
    [SerializeField] private GameObject _CommissionPrefab;
    [SerializeField] private Transform _CommissionArea;
    [SerializeField] private Text _Description;

    private List<CommissionObject> _CommissionObjects;
    private List<Commission> _Commissions;

    private Commission _Selected; // selected commission in the log

    private CanvasGroup _CanvasGroup;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();

        _CommissionObjects = new List<CommissionObject>();
        _Commissions = new List<Commission>();
        _CanvasGroup.alpha = 0.0f;
    }

    private void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            OpenExit();
        }
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            foreach (CommissionObject co in _CommissionObjects)
            {
                co.Commission.Objectives.ForEach(o => ((FetchObjective)o).UpdateItemCount());
            }
        }
    }

    public void AcceptCommission(Commission commission)
    {
        _Commissions.Add(commission);

        GameObject obj = Instantiate(_CommissionPrefab, _CommissionArea); // create object to be added to the log list

        CommissionObject cs = obj.GetComponent<CommissionObject>();
        commission.Object = cs;
        cs.Commission = commission;

        obj.GetComponent<Text>().text = commission.Title;

        _CommissionObjects.Add(cs);

        CheckCompletion();
    }

    public void AbandonCommission()
    {
        RemoveCommission(_Selected.Object);
    }

    /// <summary>
    /// remove specified commission from the log
    /// </summary>
    public void RemoveCommission(CommissionObject commObject)
    {
        if (commObject == null)
            return;

        _CommissionObjects.Remove(commObject);
        _Commissions.Remove(commObject.Commission);

        Destroy(commObject.gameObject);

        if (commObject.Commission == CommissionTracker.Instance.Commission)
            CommissionTracker.Instance.Untrack();

        _Description.text = string.Empty;
        _Selected = null;
    }

    /// <summary>
    /// track the selected commission
    /// </summary>
    public void TrackCommission()
    {
        if (_Selected == null)
            return;

        CommissionTracker.Instance.Track(_Selected);
    }

    public void UpdateSelected()
    {
        ShowDescription(_Selected);
    }

    public void ShowDescription(Commission commission)
    {
        if (commission == null)
            return;

        if (_Selected != null && _Selected != commission)
            _Selected.Object.Deselect();

        _Selected = commission;

        string objectives = "<b>Objectives</b>\n<size=20>";
        foreach (Objective obj in commission.Objectives)
        {
            objectives += obj.Type + ": " + obj.CurrentAmount + "/" + obj.Amount + "\n";
        }
        objectives += "</size>";

        string rewards = "<b>Rewards</b>\n<size=20>";
        foreach (RewardPair<string, int> reward in commission.Rewards)
        {
            rewards += reward.Amount + " " + reward.ID + "\n";
        }
        rewards += "</size>";

        string giver = "<b>Giver</b>\n<size=20>" + commission.Giver.name + "</size>";

        string completed = commission.IsCompleted ? "<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("<b>{0}</b>\n<size=20>{1}</size>\n\n{2}\n{3}\n{4}\n\n{5}", commission.Title, commission.Description, objectives, rewards, giver, completed);
    }

    public void CheckCompletion()
    {
        foreach (CommissionObject co in _CommissionObjects)
        {
            co.IsCompleted();
        }
    }

    public void OpenExit()
    {
        if (_CanvasGroup.alpha == 1.0f)
        {
            Exit();
        }
        else
        {
            _CanvasGroup.alpha = 1.0f;
            _CanvasGroup.blocksRaycasts = true;
        }
    }

    public void Exit()
    {
        if (_Selected != null)
            _Selected.Object.Deselect();

        _CanvasGroup.alpha = 0.0f;
        _CanvasGroup.blocksRaycasts = false;

        _Description.text = string.Empty;
    }

    public bool HasCommission(Commission commission)
    {
        return _Commissions.Exists(c => c.Title == commission.Title);
    }
}