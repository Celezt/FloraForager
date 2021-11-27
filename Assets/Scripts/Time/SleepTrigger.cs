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

        ConfirmMenuFactory.Instance.CreateMenu(UIStateVisibility.Instance.transform, true, 
            "Sleep? Stamina is fully replenished and all unsaved progress is saved", 
            new UnityAction(() => 
            {
                SleepSchedule.Instance.StartSleeping(false);
                SleepSchedule.Instance.OnSlept += SleptAction;

                void SleptAction(bool value)
                {
                    UIStateVisibility.Instance.Show("inventory", "player_hud");
                    SleepSchedule.Instance.OnSlept -= SleptAction;
                };
            }), 
            new UnityAction(() => 
            {
                playerInput.ActivateInput();
                UIStateVisibility.Instance.Show("inventory", "player_hud");
            }));

        _Colliding = true;
    }
}
