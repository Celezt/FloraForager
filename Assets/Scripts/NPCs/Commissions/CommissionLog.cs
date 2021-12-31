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

    [SerializeField] private ScrollRect _CommissionScrollRect;
    [SerializeField] private ScrollRect _DescriptionScrollRect;

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

        string description = "<u><b>Description</b></u>\n<size=26>" + commission.CommissionData.Description + "</size>\n";

        string objectives = "\n<u><b>Objectives</b></u>\n<size=26>";
        commission.Objectives.ForEach(o =>
        {
            objectives += o.Status + '\n';
        });
        objectives += "</size>";

        string rewards = "\n<u><b>Rewards</b></u>\n<size=26>";
        foreach (ItemAsset reward in commission.CommissionData.Rewards)
        {
            rewards += reward.Amount + " " + ItemTypeSettings.Instance.ItemNameChunk[reward.ID] + "\n";
        }
        rewards += "</size>";

        string daysLeft = commission.HasTimeLimit ? "\n<u><b>Time limit</b></u>\n<size=26>" + commission.DaysLeft.ToString() + " Days</size>\n" : string.Empty;

        string repeatable = "\n<u><b>Repeatable</b></u>\n<size=26>" + (commission.Repeatable ? "Yes" : "No") + "</size>\n";

        string giver = "\n<u><b>Giver</b></u>\n<size=26>" + commission.Giver + "</size>\n";

        string completed = commission.IsCompleted ? "\n<color=green>(Complete)</color>" : string.Empty;

        _Description.text = string.Format("{0}{1}{2}{3}{4}{5}{6}",
            description, objectives, rewards, daysLeft, repeatable, giver, completed);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_Description.GetComponent<RectTransform>());
        _DescriptionScrollRect.verticalNormalizedPosition = 1f;
    }

    public void OnOpenExit(InputAction.CallbackContext context)
    {
        if (DebugManager.IsFocused)
            return;

        if (_CanvasGroup.alpha == 1.0f)
        {
            Exit();
        }
        else
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_CommissionScrollRect.GetComponent<RectTransform>());
            _CommissionScrollRect.verticalNormalizedPosition = 1f;

            SoundPlayer.Instance.Play(_OpenSound);

            _CanvasGroup.alpha = 1.0f;
            _CanvasGroup.blocksRaycasts = true;
        }
    }

    public void Exit()
    {
        if (_Selected != null)
            _Selected.Object.Deselect();

        SoundPlayer.Instance.Play(_CloseSound);

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
