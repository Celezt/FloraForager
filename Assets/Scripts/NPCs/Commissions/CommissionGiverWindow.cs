using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class CommissionGiverWindow : Singleton<CommissionGiverWindow>
{
    [SerializeField] private GameObject _CommissionPrefab;
    [SerializeField] private Transform _CommissionArea;

    [SerializeField] private GameObject _AcceptButton;
    [SerializeField] private GameObject _BackButton;
    [SerializeField] private GameObject _CompleteButton;
    [SerializeField] private GameObject _CommissionDescription;

    [SerializeField] private Text _Title;

    private Text _Description;

    private CanvasGroup _CanvasGroup;

    private CommissionGiver _CommissionGiver; // giver assigned to this window
    private List<GameObject> _CommissionObjects; // objects in the window list
    private Commission _SelectedCommission; // selected commission in the window

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _Description = _CommissionDescription.GetComponent<Text>();

        _CommissionObjects = new List<GameObject>();
        _CanvasGroup.alpha = 0.0f;
    }

    public void ShowCommissions(CommissionGiver commissionGiver)
    {
        _CommissionGiver = commissionGiver;

        _Title.text = commissionGiver.name;

        _CommissionObjects.ForEach(o => Destroy(o));
        _CommissionObjects.Clear();

        _CommissionArea.gameObject.SetActive(true);
        _CommissionDescription.SetActive(false);

        foreach (Commission commission in commissionGiver.Commissions)
        {
            if (commission == null)
                continue;

            GameObject obj = Instantiate(_CommissionPrefab, _CommissionArea);

            Text commText = obj.GetComponent<Text>();

            commText.text = commission.Title;
            obj.GetComponent<CGCommissionObject>().Commission = commission;

            _CommissionObjects.Add(obj);

            bool hasCoom = CommissionLog.Instance.HasCommission(commission);
            if (hasCoom && commission.IsCompleted)
            {
                commText.color = Color.green;
            }
            else if (hasCoom) // fade out commissions that are already assigned
            {
                Color c = commText.color;
                c.a = 0.5f;
                commText.color = c;
            }

            foreach (Objective objective in commission.Objectives)
            {
                objective.UpdateAmount(0);
            }
        }
    }

    public void ShowCommissionInfo(Commission commission)
    {
        _SelectedCommission = commission;

        bool hasCoom = CommissionLog.Instance.HasCommission(commission);
        if (hasCoom && commission.IsCompleted)
        {
            _AcceptButton.SetActive(false);
            _CompleteButton.SetActive(true);
        }
        else if (!hasCoom)
        {
            _AcceptButton.SetActive(true);
        }

        _BackButton.SetActive(true);

        _CommissionArea.gameObject.SetActive(false);
        _CommissionDescription.SetActive(true);

        string objectives = "<b>Objectives</b>\n<size=20>";
        foreach (Objective obj in commission.Objectives)
        {
            objectives += obj.ItemID + ": " + obj.CurrentAmount + "/" + obj.Amount + "\n";
        }
        objectives += "</size>";

        string rewards = "<b>Rewards</b>\n<size=20>";
        foreach (RewardPair reward in commission.Rewards)
        {
            rewards += reward.Amount + " " + reward.ItemID + "\n";
        }
        rewards += "</size>";

        string daysLeft = "<b>Time limit</b>\n<size=20>" + commission.DaysLeft.ToString() + " Days</size>";

        string completed = commission.IsCompleted ? "<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("<b>{0}</b>\n<size=20>{1}</size>\n\n{2}\n{3}\n{4}\n\n{5}",
            commission.Title,
            commission.Description,
            objectives, rewards, daysLeft, completed);
    }

    public void Open()
    {
        _CanvasGroup.alpha = 1.0f;
        _CanvasGroup.blocksRaycasts = true;
    }

    public void Exit()
    {
        _CanvasGroup.alpha = 0.0f;
        _CanvasGroup.blocksRaycasts = false;

        _BackButton.SetActive(false);
        _AcceptButton.SetActive(false);
        _CompleteButton.SetActive(false);
    }

    /// <summary>
    /// Accept a selected commission
    /// </summary>
    public void Accept()
    {
        CommissionLog.Instance.AcceptCommission(_SelectedCommission);
        Back();
    }

    /// <summary>
    /// Go back to the commissions list on the commissions giver window
    /// </summary>
    public void Back()
    {
        _BackButton.SetActive(false);
        _AcceptButton.SetActive(false);
        _CompleteButton.SetActive(false);

        ShowCommissions(_CommissionGiver);
    }

    /// <summary>
    /// Complete a selected commission in the commissions giver window
    /// </summary>
    public void Complete()
    {
        if (!_SelectedCommission.IsCompleted)
            return;

        for (int i = 0; i < _CommissionGiver.Commissions.Length; ++i)
        {
            if (_SelectedCommission == _CommissionGiver.Commissions[i])
            {
                _CommissionGiver.Commissions[i] = null;
            }
        }

        _SelectedCommission.Complete();

        CommissionLog.Instance.RemoveCommission(_SelectedCommission.Object);

        Back();
    }
}
