using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField]
    private Image _StaminaSlider;

    private PlayerStamina _PlayerStamina;

    private void Awake()
    {
        _PlayerStamina = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerStamina>();
    }

    public void LateUpdate()
    {
        _StaminaSlider.fillAmount = (_PlayerStamina.Stamina / _PlayerStamina.MaxStamina);
    }
}
