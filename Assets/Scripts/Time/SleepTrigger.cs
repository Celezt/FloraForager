using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SleepTrigger : MonoBehaviour
{
    [SerializeField]
    private string _SleepSound = "door_hit_sleep";

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

        UIStateVisibility.Instance.Hide("inventory", "player_hud");
        playerInput.DeactivateInput();

        ConfirmMenuFactory.Instance.CreateMenu(UIStateVisibility.Instance.transform, 10, true,
            "Sleep? Stamina is fully replenished and all unsaved progress is saved", 
            new UnityAction(() => 
            {
                SoundPlayer.Instance.Play(_SleepSound);

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
                UIStateVisibility.Instance.Show("inventory", "player_hud");
                playerInput.ActivateInput();
            }));

        _Colliding = true;
    }
}
