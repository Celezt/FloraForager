using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;
using IngameDebugConsole;

public class SleepSchedule : Singleton<SleepSchedule>
{
    [SerializeField, Range(0.0f, 24.0f)]
    private float _MorningTime = 6.0f; // morning time in 00:00-24:00 range
    [SerializeField, Range(0.0f, 24.0f)] 
    private float _NightTime = 22.0f;
    [Space(5)]
    [SerializeField, Min(0)] 
    private float _SleepTime = 4.0f;

    private PlayerStamina _PlayerStamina;
    private PlayerMovement _PlayerMovement;
    private InteractBehaviour _InteractableArea;
    private Rigidbody _PlayerRigidbody;

    private bool _IsSleeping = false;  // if the player is currently sleeping
    private float _NightToMorning = 0; // total time in hours for player to sleep

    public System.Action OnSlept = delegate { };

    public float MorningTime => _MorningTime;
    public float NightTime => _NightTime;
    public float NightToMorning => _NightToMorning;

    public bool IsSleeping => _IsSleeping;

    private void Awake()
    {
        GameObject player = PlayerInput.GetPlayerByIndex(0).gameObject;

        _PlayerMovement = player.GetComponent<PlayerMovement>();
        _PlayerStamina = player.GetComponent<PlayerStamina>();
        _InteractableArea = player.GetComponent<InteractBehaviour>();
        _PlayerRigidbody = player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _NightToMorning = (24.0f + (_MorningTime - _NightTime)) % 24.0f;

        DebugLogConsole.AddCommandInstance("player.sleep", "Activate sleep mode", nameof(ConsoleStartSleeping), this);
    }

    private void ConsoleStartSleeping() => StartSleeping();
    public bool StartSleeping()
    {
        if (_IsSleeping)
            return false;

        _IsSleeping = true;
        StartCoroutine(Sleep(GameTime.Instance.CurrentTime));

        return true;
    }

    private IEnumerator Sleep(float currentTime)
    {
        // sleep

        FadeScreen.Instance.StartFadeIn(_SleepTime);

        _PlayerMovement.enabled = false;
        _InteractableArea.enabled = false;
        _PlayerRigidbody.isKinematic = true;

        yield return new WaitForSeconds(_SleepTime);

        // wake up

        FadeScreen.Instance.StartFadeOut(2.0f);

        _PlayerMovement.enabled = true;
        _InteractableArea.enabled = true;
        _PlayerRigidbody.isKinematic = false;

        OnSlept.Invoke();

        _PlayerStamina.Recover();
        FloraMaster.Instance.Notify();
        CommissionList.Instance.Notify();

        GameTime.Instance.AccelerateTime(currentTime, TimeToMorning(currentTime));

        _IsSleeping = false;
    }

    public float TimeToNight(float time)
    {
        return (24.0f + (_NightTime - time)) % 24.0f;
    }

    public float TimeToMorning(float time)
    {
        return (24.0f + (_MorningTime - time)) % 24.0f;
    }
}
