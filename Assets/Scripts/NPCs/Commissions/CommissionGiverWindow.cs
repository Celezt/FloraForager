using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class CommissionGiverWindow : Singleton<CommissionGiverWindow>
{
    [SerializeField] private GameObject _CommissionPrefab;
    [SerializeField] private Transform _CommissionArea;

    [SerializeField] private GameObject _AcceptButton;
    [SerializeField] private GameObject _BackButton;
    [SerializeField] private GameObject _CompleteButton;
    [SerializeField] private GameObject _DescriptionArea;

    [SerializeField] private TMP_Text _Title;
    [SerializeField] private TMP_Text _Description;
    [SerializeField] private ScrollRect _ScrollRect;

    private CanvasGroup _CanvasGroup;

    private NPC _CommissionGiver; // giver assigned to this window
    private List<GameObject> _CommissionObjects; // objects in the window list
    private Commission _SelectedCommission; // selected commission in the window

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();

        _CommissionObjects = new List<GameObject>();
        _CanvasGroup.alpha = 0.0f;
    }

    public void ShowCommissions(NPC commissionGiver)
    {
        _CommissionGiver = commissionGiver;

        _Title.text = commissionGiver.Name;

        _CommissionObjects.ForEach(o => Destroy(o));
        _CommissionObjects.Clear();

        _CommissionArea.gameObject.SetActive(true);
        _DescriptionArea.SetActive(false);

        _ScrollRect.content = _CommissionArea.GetComponent<RectTransform>();
        _ScrollRect.viewport = null;

        foreach (Commission commission in commissionGiver.Commissions)
        {
            if (commission == null)
                continue;

            GameObject obj = Instantiate(_CommissionPrefab, _CommissionArea);

            TMP_Text commText = obj.GetComponent<TMP_Text>();

            commText.text = commission.CommissionData.Title;
            obj.GetComponent<CGCommissionObject>().Commission = commission;

            _CommissionObjects.Add(obj);

            bool hasComm = CommissionLog.Instance.HasCommission(commission);
            bool enoughRelation = (int)_CommissionGiver.Relation.Relation >= (int)commission.CommissionData.MinRelation;

            if (hasComm && commission.IsCompleted)
            {
                commText.color = Color.green;
            }
            else if (hasComm || !enoughRelation) // fade out commissions that are already assigned
            {
                Color c = commText.color;
                c.a = 0.5f;
                commText.color = c;
            }
        }
    }

    public void ShowDescription(Commission commission)
    {
        _SelectedCommission = commission;

        bool hasComm = CommissionLog.Instance.HasCommission(commission);
        bool enoughRelation = (int)_CommissionGiver.Relation.Relation >= (int)commission.CommissionData.MinRelation;

        if (hasComm && commission.IsCompleted)
        {
            _AcceptButton.SetActive(false);
            _CompleteButton.SetActive(true);
        }
        else if (!hasComm && enoughRelation)
        {
            _AcceptButton.SetActive(true);
        }

        _BackButton.SetActive(true);

        _CommissionArea.gameObject.SetActive(false);
        _DescriptionArea.SetActive(true);

        _ScrollRect.content = _Description.GetComponent<RectTransform>();
        _ScrollRect.viewport = _DescriptionArea.GetComponent<RectTransform>();

        string objectives = "<b>Objectives</b>\n<size=20>";
        commission.Objectives.ForEach(o =>
        {
            objectives += o.Status + '\n';
        });
        objectives += "</size>";

        string rewards = "<b>Rewards</b>\n<size=20>";
        foreach (RewardPair reward in commission.CommissionData.Rewards)
        {
            rewards += reward.Amount + " " + ItemTypeSettings.Instance.ItemNameChunk[reward.ItemID] + "\n";
        }
        rewards += "</size>";

        string daysLeft = "<b>Time limit</b>\n<size=20>" + commission.DaysLeft.ToString() + " Days</size>";

        string minRelation = "<b>Minimum Relations</b>\n<size=20>" + commission.CommissionData.MinRelation + "</size>";

        string completed = commission.IsCompleted ? "<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("<b>{0}</b>\n<size=20>{1}</size>\n\n{2}\n{3}\n{4}\n\n{5}\n\n{6}",
            commission.CommissionData.Title,
            commission.CommissionData.Description,
            objectives, rewards, daysLeft, minRelation, completed);
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
        CommissionLog.Instance.Accept(_SelectedCommission);
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
        CommissionLog.Instance.Remove(_SelectedCommission);

        Back();
    }
}
