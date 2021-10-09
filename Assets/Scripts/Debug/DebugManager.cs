using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using IngameDebugConsole;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Manage everything related to debug mode.
/// </summary>
public class DebugManager : Singleton<DebugManager>
{
    /// <summary>
    /// If debug mode is on or off.
    /// </summary>
    public static bool DebugMode = false;
    /// <summary>
    /// Callbacks for debug mode on events.
    /// </summary>
    public static event DebugHandler OnDebugModeOn = delegate { };
    /// <summary>
    /// Callbacks for debug mode off events.
    /// </summary>
    public static event DebugHandler OnDebugModeOff = delegate { };

    public delegate void DebugHandler();

    private InputField _inputField;

    private PlayerActionHandle _playerHandle1;
    private PlayerActionHandle _playerHandle2;
    private PlayerActionHandle _playerHandle3;

    private bool _isFocus;

    private void Awake()
    {
        DebugLogManager.Instance.OnLogWindowShown += new System.Action(() => 
        { 
            DebugMode = true;
            StartCoroutine(IsCommandConsoleFocused());
            OnDebugModeOn.Invoke();
        });
        DebugLogManager.Instance.OnLogWindowHidden += new System.Action(() => 
        { 
            DebugMode = false;
            OnDebugModeOff.Invoke();
        });

        _inputField = DebugLogManager.Instance.transform.Find("DebugLogWindow").Find("CommandInputField").GetComponent<InputField>();
    }

    private void OnDisable()
    {
        DebugMode = false;
    }

    private IEnumerator IsCommandConsoleFocused()
    {
        bool oldIsFocused = !_inputField.isFocused;
        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);
        PlayerMovement movement = playerInput.GetComponent<PlayerMovement>();
        InteractBehaviour interactableBehaviour = playerInput.GetComponent<InteractBehaviour>();
        UseBehaviour useBehaviour = playerInput.GetComponent<UseBehaviour>();

        while (DebugMode)
        {
            bool isFocused = _inputField.isFocused;

            if (isFocused != oldIsFocused)
            {
                if (isFocused)
                {
                    if (!_isFocus)
                    {
                        _isFocus = true;
                        _playerHandle1 = movement.Inputs.AddSharedDisable();
                        _playerHandle2 = interactableBehaviour.Inputs.AddSharedDisable();
                        _playerHandle3 = useBehaviour.Inputs.AddSharedDisable();
                    }
                }
                else
                {
                    _isFocus = false;
                    movement.Inputs.RemoveSharedDisable(_playerHandle1);
                    interactableBehaviour.Inputs.RemoveSharedDisable(_playerHandle2);
                    useBehaviour.Inputs.RemoveSharedDisable(_playerHandle3);
                }
            }

            oldIsFocused = isFocused;

            yield return null;
        }

        movement.Inputs.RemoveSharedDisable(_playerHandle1);
        interactableBehaviour.Inputs.RemoveSharedDisable(_playerHandle2);
        useBehaviour.Inputs.RemoveSharedDisable(_playerHandle3);
    }
}
