using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SleepTrigger : MonoBehaviour
{
    private bool _Colliding = false;

    private void Awake()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        _Colliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (SleepSchedule.Instance.IsSleeping || FadeScreen.Instance.IsActive || _Colliding || !other.CompareTag("Player"))
            return;

        PlayerInput playerInput = other.GetComponent<PlayerInput>();

        playerInput.DeactivateInput();

        UIStateVisibility.Instance.Hide("inventory", "player_hud");
        UIStateVisibility.Instance.Show("confirm_menu");

        ConfirmMenuFactory.Instance.CreateMenu(UIStateVisibility.Instance.transform, true, 
            "Sleep? Stamina is fully replenished and all unsaved progress is saved", 
            new UnityAction(() => 
            {
                SleepSchedule.Instance.StartSleeping(false);

                UIStateVisibility.Instance.Show("inventory", "player_hud");
                UIStateVisibility.Instance.Hide("confirm_menu");
            }), 
            new UnityAction(() => 
            {
                playerInput.ActivateInput();

                UIStateVisibility.Instance.Show("inventory", "player_hud");
                UIStateVisibility.Instance.Hide("confirm_menu");
            }));

        _Colliding = true;
    }
}
