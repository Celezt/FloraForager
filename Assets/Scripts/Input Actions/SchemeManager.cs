using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SchemeManager : MonoBehaviour
{
    [HideInInspector] public InputControlScheme CurrentScheme;

    private PlayerAction _inputs;

    public void OnDeviceChanged(PlayerInput input)
    {
        CurrentScheme = input.user.controlScheme.Value;
    }

    private void Awake()
    {
        _inputs = new PlayerAction();
    }

    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }
}
