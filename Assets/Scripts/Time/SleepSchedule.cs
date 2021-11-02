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

    private PlayerInput _PlayerInput;
    private PlayerStamina _PlayerStamina;

    private bool _IsSleeping = false;  // if the player is currently sleeping
    private float _NightToMorning = 0; // total time in hours for player to sleep

    public System.Action OnSlept = delegate { };

    public float MorningTime => _MorningTime;
    public float NightTime => _NightTime;
    public float NightToMorning => _NightToMorning;

    public bool IsSleeping => _IsSleeping;

    private void Awake()
    {
        _PlayerInput = PlayerInput.GetPlayerByIndex(0);
        _PlayerStamina = _PlayerInput.GetComponent<PlayerStamina>();
    }

    private void Start()
    {
        _NightToMorning = (24.0f + (_MorningTime - _NightTime)) % 24.0f;

        DebugLogConsole.AddCommandInstance("player.sleep", "Activate sleep mode", nameof(ConsoleStartSleeping), this);
    }

    private void ConsoleStartSleeping() => StartSleeping(false);
    public bool StartSleeping(bool penalty)
    {
        if (_IsSleeping)
            return false;

        _IsSleeping = true;
        StartCoroutine(Sleep(penalty));

        return true;
    }

    private IEnumerator Sleep(bool penalty)
    {
        // sleep

        float currentTime = GameTime.Instance.CurrentTime;

        _PlayerInput.DeactivateInput();

        FadeScreen.Instance.StartFadeIn(_SleepTime);
        yield return new WaitForSeconds(_SleepTime);
        FadeScreen.Instance.StartFadeOut(2.5f);

        // wake up

        _PlayerInput.ActivateInput();

        _PlayerStamina.Recover(penalty);
        FloraMaster.Instance.Notify();
        CommissionList.Instance.Notify();

        GameTime.Instance.AccelerateTime(currentTime, TimeToMorning(currentTime));

        _IsSleeping = false;

        OnSlept.Invoke();
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
