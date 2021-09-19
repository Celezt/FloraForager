using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using IngameDebugConsole;

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

    private void Awake()
    {
        DebugLogManager.Instance.OnLogWindowShown += new System.Action(() => 
        { 
            DebugMode = true;
            OnDebugModeOn.Invoke();
        });
        DebugLogManager.Instance.OnLogWindowHidden += new System.Action(() => 
        { 
            DebugMode = false;
            OnDebugModeOff.Invoke();
        });
    }
}
