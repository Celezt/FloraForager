using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using MyBox;

public class ConfirmMenu : Singleton<ConfirmMenu>
{
    [SerializeField]
    private TMP_Text _WarningText;
    [SerializeField] 
    private Button _YesButton = null;
    [SerializeField] 
    private Button _NoButton = null;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetMenu(string warningText, UnityAction yesAction, UnityAction noAction)
    {
        SetWarningText(warningText);
        YesAction(yesAction);
        NoAction(noAction);
    }

    public void SetWarningText(string warningText)
    {
        _WarningText.text = warningText;
    }

    public void YesAction(UnityAction yesAction)
    {
        _YesButton.onClick.RemoveAllListeners();
        _YesButton.onClick.AddListener(yesAction);
    }

    public void NoAction(UnityAction noAction)
    {
        _NoButton.onClick.RemoveAllListeners();
        _NoButton.onClick.AddListener(noAction);
    }
}
