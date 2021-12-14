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
    public static bool DebugMode => _debugMode;
    /// <summary>
    /// If command field is in focus.
    /// </summary>
    public static bool IsFocused => _isFocused;
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

    private static bool _isFocused;
    private static bool _debugMode;

    private void Awake()
    {
        DebugLogManager.Instance.OnLogWindowShown += new System.Action(() => 
        {
            _debugMode = true;
            StartCoroutine(IsCommandConsoleFocused());
            OnDebugModeOn.Invoke();
        });
        DebugLogManager.Instance.OnLogWindowHidden += new System.Action(() => 
        {
            _debugMode = false;
            OnDebugModeOff.Invoke();
        });

        _inputField = DebugLogManager.Instance.transform.Find("DebugLogWindow").Find("CommandInputField").GetComponent<InputField>();
    }

    private void OnDisable()
    {
        _debugMode = false;
    }

    private IEnumerator IsCommandConsoleFocused()
    {
        bool oldIsFocused = !_inputField.isFocused;
        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);

        if (playerInput == null)
            yield break;

        while (_debugMode)
        {
            bool isFocused = _inputField.isFocused;

            if (isFocused != oldIsFocused)
            {
                if (isFocused)
                {
                    if (!_isFocused)
                    {
                        _isFocused = true;
                        playerInput.DeactivateInput();
                    }
                }
                else
                {
                    _isFocused = false;
                    playerInput.ActivateInput();
                }
            }

            oldIsFocused = isFocused;

            yield return null;
        }

        _isFocused = false;
        playerInput.ActivateInput();

        yield break;
    }
}
