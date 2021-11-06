using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private CanvasGroup _CanvasGroup;

    private PlayerAction _PlayerAction;

    private float _CurrentTimeScale;
    private string[] _ShownStates;

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
        _CanvasGroup.alpha = 1.0f - _CanvasGroup.alpha;
        _CanvasGroup.blocksRaycasts = !_CanvasGroup.blocksRaycasts;

        UIStateVisibility.Instance.Hide("confirm_menu");

        if (_CanvasGroup.alpha == 1.0f)
        {
            _CurrentTimeScale = Time.timeScale;
            _ShownStates = UIStateVisibility.Instance.GetShownStates();

            Time.timeScale = 0.0f;
            UIStateVisibility.Instance.Hide("inventory", "player_hud");
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

    public void OnQuit()
    {
        UIStateVisibility.Instance.Show("confirm_menu");

        ConfirmMenu.Instance.SetMenu("Return to Main Menu?", 
        new UnityAction(() => 
        {
            SceneManager.LoadScene("MainMenu");
        }), 
        new UnityAction(() => 
        {
            UIStateVisibility.Instance.Hide("confirm_menu");
        }));
    }
}
