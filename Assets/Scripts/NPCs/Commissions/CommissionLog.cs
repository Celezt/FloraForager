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

    private List<CommissionObject> _CommissionObjects = new List<CommissionObject>();
    private List<Commission> _Commissions = new List<Commission>();

    private Commission _Selected; // selected commission in the log

    private CanvasGroup _CanvasGroup;
    private PlayerAction _PlayerAction;

    public List<Commission> Commissions => _Commissions;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _PlayerAction = new PlayerAction();
    }

    private void OnEnable()
    {
        _PlayerAction.Enable();
        _PlayerAction.Ground.CommissionLog.started += OnOpenExit;
    }

    private void OnDisable()
    {
        _PlayerAction.Disable();
        _PlayerAction.Ground.CommissionLog.started -= OnOpenExit;
    }

    public void AcceptCommission(Commission commission)
    {
        _Commissions.Add(commission);

        GameObject obj = Instantiate(_CommissionPrefab, _CommissionArea); // create object to be added to the log list

        CommissionObject cs = obj.GetComponent<CommissionObject>();
        commission.Object = cs;
        cs.Commission = commission;

        obj.GetComponent<Text>().text = commission.Data.Title;

        _CommissionObjects.Add(cs);

        CheckCompletion();
    }

    public void AbandonCommission()
    {
        _Selected.RemoveWithPenalty();
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
        commission.Data.Objectives.ForEach(o => 
        { 
            objectives += o.Status + '\n'; 
        });
        objectives += "</size>";

        string rewards = "<b>Rewards</b>\n<size=20>";
        foreach (RewardPair reward in commission.Data.Rewards)
        {
            rewards += reward.Amount + " " + reward.ItemID + "\n";
        }
        rewards += "</size>";

        string daysLeft = "<b>Time limit</b>\n<size=20>" + commission.DaysLeft.ToString() + " Days</size>";

        string giver = "<b>Giver</b>\n<size=20>" + commission.Giver.Name + "</size>";

        string completed = commission.IsCompleted ? "<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("<b>{0}</b>\n<size=20>{1}</size>\n\n{2}\n{3}\n{4}\n\n{5}\n\n{6}", 
            commission.Data.Title, 
            commission.Data.Description, 
            objectives, rewards, daysLeft, giver, completed);
    }

    public void CheckCompletion()
    {
        foreach (CommissionObject co in _CommissionObjects)
        {
            co.IsCompleted();
        }
    }

    public void OnOpenExit(InputAction.CallbackContext context)
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

    /// <summary>
    /// notify all commissions that a day has passed
    /// </summary>
    public void Notify()
    {
        _Commissions.ForEach(c => c.DayPassed());
    }

    public bool HasCommission(Commission commission)
    {
        return _Commissions.Exists(c => c.Data.Title == commission.Data.Title);
    }
}
