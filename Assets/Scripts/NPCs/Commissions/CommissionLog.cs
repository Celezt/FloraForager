using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using MyBox;

/// <summary>
/// Keeps track of and manages all of the accepted commissions
/// </summary>
public class CommissionLog : Singleton<CommissionLog>
{
    [SerializeField] private GameObject _CommissionPrefab;
    [SerializeField] private Transform _CommissionArea;
    [SerializeField] private TMP_Text _Description;

    [Space(10)]
    [SerializeField]
    private string _OpenSound = "open_window";
    [SerializeField]
    private string _CloseSound = "close_window";

    private List<CommissionObject> _CommissionObjects = new List<CommissionObject>();

    private Commission _Selected; // selected commission in the log

    private CanvasGroup _CanvasGroup;
    private PlayerAction _PlayerAction;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        _PlayerAction.Disable();
        _PlayerAction.Ground.CommissionLog.started -= OnOpenExit;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Accept(Commission commission)
    {
        CommissionObject commObj = Instantiate(_CommissionPrefab, _CommissionArea).GetComponent<CommissionObject>();

        commission.Object = commObj;
        commObj.Commission = commission;

        if (_CommissionObjects.Count % 2 != 0)
            commObj.Background.color = Color.clear;

        commObj.Text.text = commission.CommissionData.Title;

        CommissionList.Instance.Add(commission);
        _CommissionObjects.Add(commObj);

        commission.Objectives.ForEach(o => o.Accepted());

        CheckCompletion();
    }

    public void Abandon()
    {
        if (_Selected == null)
            return;

        _Selected.Penalty();
        Remove(_Selected);
    }

    /// <summary>
    /// remove specified commission from the log
    /// </summary>
    public void Remove(Commission commission)
    {
        if (commission == null || commission.Object == null)
            return;

        _CommissionObjects.Remove(commission.Object);
        CommissionList.Instance.Remove(commission);

        Destroy(commission.Object.gameObject);

        if (commission == CommissionTracker.Instance.Commission)
            CommissionTracker.Instance.Untrack();

        _Description.text = string.Empty;
        _Selected = null;

        commission.Objectives.ForEach(o => o.Removed());
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

    public void CheckCompletion()
    {
        foreach (CommissionObject co in _CommissionObjects)
        {
            co.IsCompleted();
        }
    }

    public void ShowDescription(Commission commission)
    {
        if (commission == null)
            return;

        if (_Selected != null && _Selected != commission)
            _Selected.Object.Deselect();

        _Selected = commission;

        string objectives = "<u><b>Objectives</b></u>\n<size=22>";
        commission.Objectives.ForEach(o =>
        {
            objectives += o.Status + '\n';
        });
        objectives += "</size>";

        string rewards = "<u><b>Rewards</b></u>\n<size=22>";
        foreach (ItemAsset reward in commission.CommissionData.Rewards)
        {
            rewards += reward.Amount + " " + ItemTypeSettings.Instance.ItemNameChunk[reward.ID] + "\n";
        }
        rewards += "</size>";

        string daysLeft = "<u><b>Time limit</b></u>\n<size=22>" + commission.DaysLeft.ToString() + " Days</size>";

        string giver = "<u><b>Giver</b></u>\n<size=22>" + commission.Giver + "</size>";

        string repeatable = "<u><b>Repeatable</b></u>\n<size=22>" + (commission.Repeatable ? "Yes" : "No") + "</size>";

        string completed = commission.IsCompleted ? "<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("<u><b>Description</b></u>\n<size=22>{0}</size>\n\n{1}\n{2}\n{3}\n\n{4}\n\n{5}\n\n{6}",
            commission.CommissionData.Description,
            objectives, rewards, daysLeft, giver, repeatable, completed);
    }

    public void OnOpenExit(InputAction.CallbackContext context)
    {
        if (DebugManager.IsFocused)
            return;

        if (_CanvasGroup.alpha == 1.0f)
        {
            SoundPlayer.Instance.Play(_CloseSound);

            Exit();
        }
        else
        {
            SoundPlayer.Instance.Play(_OpenSound);

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        foreach (Commission commission in CommissionList.Instance.Commissions)
        {
            CommissionObject commObj = Instantiate(_CommissionPrefab, _CommissionArea).GetComponent<CommissionObject>();

            commission.Object = commObj;
            commObj.Commission = commission;

            if (_CommissionObjects.Count % 2 != 0)
                commObj.Background.color = Color.clear;

            commObj.Text.text = commission.CommissionData.Title;

            _CommissionObjects.Add(commObj);

            commission.Objectives.ForEach(o => o.Accepted());
        }

        CheckCompletion();
    }
}
