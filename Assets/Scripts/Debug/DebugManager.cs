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
        PlayerInventoryUI[] playerInventoryUI = FindObjectsOfType<PlayerInventoryUI>();

        PlayerActionHandle[] playerHandles = new PlayerActionHandle[playerInventoryUI.Length + 3];

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
                        playerHandles[0] = movement.Inputs.AddSharedDisable();
                        playerHandles[1] = interactableBehaviour.Inputs.AddSharedDisable();
                        playerHandles[2] = useBehaviour.Inputs.AddSharedDisable();
                        for (int i = 0; i < playerInventoryUI.Length; i++)
                            playerHandles[i + 3] = playerInventoryUI[i].PlayerAction.AddSharedDisable();
                    }
                }
                else
                {
                    _isFocus = false;
                    for (int i = 0; i < playerHandles.Length; i++)
                        if(!playerHandles[0].IsEmpty)
                            playerHandles[i].RemoveSharedDisable();
                }
            }

            oldIsFocused = isFocused;

            yield return null;
        }

        for (int i = 0; i < playerHandles.Length; i++)
            if (!playerHandles[0].IsEmpty)
                playerHandles[i].RemoveSharedDisable();
    }
}
