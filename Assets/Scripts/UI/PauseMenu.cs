using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _Controls;
    [SerializeField]
    private GameObject _Back;

    private CanvasGroup _CanvasGroup;

    private PlayerAction _PlayerAction;

    private float _CurrentTimeScale;
    private string[] _ShownStates;

    private GameObject _ConfirmMenu;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _PlayerAction = new PlayerAction();
    }

    private void Start()
    {
        _CanvasGroup.alpha = 0.0f;
        _CanvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        _PlayerAction.Enable();
        _PlayerAction.Ground.Pause.started += OnPause;
    }

    private void OnDisable()
    {
        _PlayerAction.Disable();
        _PlayerAction.Ground.Pause.started -= OnPause;
    }

    private void TogglePause()
    {
        _Controls.SetActive(false);
        _Back.SetActive(false);

        _CanvasGroup.alpha = 1.0f - _CanvasGroup.alpha;
        _CanvasGroup.blocksRaycasts = !_CanvasGroup.blocksRaycasts;

        if (_ConfirmMenu != null)
        {
            Destroy(_ConfirmMenu);
            _ConfirmMenu = null;
        }

        if (_CanvasGroup.alpha > float.Epsilon)
        {
            _CurrentTimeScale = Time.timeScale;
            _ShownStates = UIStateVisibility.Instance.GetShownStates();

            Time.timeScale = 0.0f;
            UIStateVisibility.Instance.Hide("player_hud", "inventory", "world_info", "commission_log", "commission_giver");
        }
        else
        {
            Time.timeScale = _CurrentTimeScale;
            UIStateVisibility.Instance.Show(_ShownStates);
        }
    }

    public void OnPause(InputAction.CallbackContext callback)
    {
        TogglePause();
    }

    public void OnResume()
    {
        TogglePause();
    }
    public void OnControls()
    {
        _Controls.SetActive(true);
        _Back.SetActive(true);
    }
    public void OnBack()
    {
        _Controls.SetActive(false);
        _Back.SetActive(false);
    }
    public void OnQuit()
    {
        if (_ConfirmMenu != null)
            return;

        _ConfirmMenu = ConfirmMenuFactory.Instance.CreateMenu("Return to Main Menu? All unsaved progress will be lost", 95,
            new UnityAction(() => 
            {
                LoadScene.Instance.LoadSceneByName("MainMenu");
            }), 
            new UnityAction(() => { }));
    }
}
