using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using System.Linq;
using Sirenix.OdinInspector;

public class CMFCamHandler : MonoBehaviour
{
    public int PlayerIndex = 0;
    [Required]
    public CinemachineFreeLook CM;
    [Required]
    public InputActionReference XYAxis;
    public float YSpeed = 2;

    private InputAction _cachedAction;
    private float _yAxis;
    private float _yLerp;

    private void OnEnable()
    {
        InputUser user = InputUser.all[0];
        _cachedAction = user.actions.First(x => x.id == XYAxis.action.id);

        _cachedAction.performed += Action;

        _yAxis = CM.m_YAxis.Value;
    }

    private void OnDisable()
    {
        _cachedAction.performed -= Action;
    }

    private void Action(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        _yAxis = Mathf.Clamp01(_yAxis + value.y);
    }

    private void LateUpdate()
    {
        float oldYLerp = _yLerp;
        _yLerp = Mathf.Lerp(_yLerp, _yAxis, YSpeed * Time.deltaTime);

        if (oldYLerp != _yLerp)         // Only updated on change.
            CM.m_YAxis.Value = _yLerp;
    }
}
