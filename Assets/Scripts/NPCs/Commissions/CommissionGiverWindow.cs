using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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

    [SerializeField] private string _acceptSound = string.Empty;
    [SerializeField] private string _completeSound = "trophy";

    private CanvasGroup _CanvasGroup;

    private NPC _CommissionGiver; // giver assigned to this window
    private List<GameObject> _CommissionObjects; // objects in the window list
    private Commission _SelectedCommission; // selected commission in the window

    public bool Opened => _CanvasGroup.alpha > 0.0f;

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

            CGCommissionObject commObject = Instantiate(_CommissionPrefab, _CommissionArea).GetComponent<CGCommissionObject>();

            if (_CommissionObjects.Count % 2 != 0)
                commObject.Background.color = Color.clear;

            commObject.Text.text = commission.CommissionData.Title;
            commObject.Commission = commission;

            _CommissionObjects.Add(commObject.gameObject);

            bool hasComm = CommissionList.Instance.HasCommission(commission);
            bool enoughRelation = (int)_CommissionGiver.Relation.Relation >= (int)commission.CommissionData.MinRelation;

            if (hasComm && commission.IsCompleted)
            {
                commObject.IsCompleted();
            }
            else if (hasComm || !enoughRelation) // fade out commissions that are already assigned
            {
                commObject.IsUnavailable();
            }
        }
    }

    public void ShowDescription(Commission commission)
    {
        _SelectedCommission = commission;

        bool hasComm = CommissionList.Instance.HasCommission(commission);
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

        string objectives = "<u><b>Objectives</b></u>\n<size=22>";
        commission.Objectives.ForEach(o =>
        {
            objectives += o.Objective + '\n';
        });
        objectives += "</size>";

        string rewards = "<u><b>Rewards</b></u>\n<size=22>";
        foreach (ItemAsset reward in commission.CommissionData.Rewards)
        {
            rewards += reward.Amount + " " + ItemTypeSettings.Instance.ItemNameChunk[reward.ID] + "\n";
        }
        rewards += "</size>";

        string daysLeft = "<u><b>Time limit</b></u>\n<size=22>" + commission.CommissionData.TimeLimit.ToString() + " Days</size>";

        string minRelation = "<u><b>Min Relations</b></u>\n<size=22>" + commission.CommissionData.MinRelation + "</size>";

        string repeatable = "<u><b>Repeatable</b></u>\n<size=22>" + (commission.Repeatable ? "Yes" : "No") + "</size>";

        string completed = commission.IsCompleted ? "<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("<u><b>Description</b></u>\n<size=22>{0}</size>\n\n{1}\n{2}\n{3}\n\n{4}\n\n{5}\n\n{6}",
            commission.CommissionData.Description,
            objectives, rewards, daysLeft, minRelation, repeatable, completed);
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

        StartDialogue(_SelectedCommission.Giver, _SelectedCommission.CommissionData.AcceptDialogue);
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

        _SelectedCommission.Complete();
        CommissionLog.Instance.Remove(_SelectedCommission);

        if (!_SelectedCommission.Repeatable)
        {
            for (int i = _CommissionGiver.Commissions.Length - 1; i >= 0; --i)
            {
                if (_SelectedCommission == _CommissionGiver.Commissions[i])
                {
                    _CommissionGiver.RemoveCommission(i);
                    break;
                }
            }
        }
        else
        {
            // reinitialize commission
            for (int i = _CommissionGiver.Commissions.Length - 1; i >= 0; --i)
            {
                if (_SelectedCommission == _CommissionGiver.Commissions[i])
                {
                    _CommissionGiver.Commissions[i] = new Commission(_SelectedCommission.CommissionData);
                    break;
                }
            }
        }

        SoundPlayer.Instance.Play(_completeSound);

        Back();

        StartDialogue(_SelectedCommission.Giver, _SelectedCommission.CommissionData.CompleteDialogue);
    }

    private void StartDialogue(string giver, Dictionary<string, string> dialogueAction)
    {
        if (!dialogueAction.TryGetValue(giver.ToLower(), out string dialogue))
            return;

        if (string.IsNullOrWhiteSpace(dialogue))
            return;

        NPCBehaviour npc;
        if ((npc = NPCManager.Instance.GetObject(giver)) != null)
            npc.StartDialogue(0, dialogue);
    }
}
