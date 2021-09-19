using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

/// <summary>
/// Manage everything related to debug mode.
/// </summary>
public class DebugManager : Singleton<DebugManager> 
{
    public static bool DebugMode = false;

    [SerializeField] private InputAction _toggleBinding = new InputAction("Toggle Binding", type: InputActionType.Button, binding: "<Keyboard>/backquote", expectedControlType: "Button");

    private void Awake()
    {
		_toggleBinding.performed += (context) =>
		{
            DebugMode = !DebugMode;
        };
	}

    private void OnEnable()
    {
        _toggleBinding.Enable();
    }

    private void OnDisable()
    {
        _toggleBinding.Disable();
    }
}
