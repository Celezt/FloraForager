using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public class SleepSchedule : Singleton<SleepSchedule>
{
    [SerializeField, Range(0.0f, 24.0f)] private float _MorningTime = 6.0f; // morning time in 00:00-24:00 range
    [SerializeField, Range(0.0f, 24.0f)] private float _NightTime = 22.0f;
    [Space(5)]
    [SerializeField, Min(0)] private float _SleepTime = 1.0f;
    [SerializeField] private GameObject _Player;

    private PlayerMovement _PlayerMovement;
    private InteractableArea _InteractableArea;
    private Rigidbody _PlayerRigidbody;

    private bool _IsSleeping = false;    // if the player is currently sleeping
    private float _NightToMorning = 0; // total time in hours for player to sleep

    public float MorningTime => _MorningTime;
    public float NightTime => _NightTime;
    public float NightToMorning => _NightToMorning;

    public bool IsSleeping => _IsSleeping;

    private void Awake()
    {
        if (_Player == null)
            Debug.LogError("need reference to current Player in scene");

        _PlayerMovement = _Player.GetComponent<PlayerMovement>();
        _InteractableArea = _Player.GetComponent<InteractableArea>();
        _PlayerRigidbody = _Player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _NightToMorning = (24.0f + (_MorningTime - _NightTime)) % 24.0f;
    }

    public bool StartSleeping(PlayerStamina playerStamina, float currentTime)
    {
        if (_IsSleeping)
            return false;

        _IsSleeping = true;
        StartCoroutine(Sleep(playerStamina, currentTime));

        return true;
    }

    private IEnumerator Sleep(PlayerStamina playerStamina, float currentTime)
    {
        // sleep

        _PlayerMovement.enabled = false;
        _InteractableArea.enabled = false;
        _PlayerRigidbody.isKinematic = true;

        yield return new WaitForSeconds(_SleepTime);

        // wake up

        _PlayerMovement.enabled = true;
        _InteractableArea.enabled = true;
        _PlayerRigidbody.isKinematic = false;

        playerStamina.Recover();
        FloraMaster.Instance.Notify();

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
