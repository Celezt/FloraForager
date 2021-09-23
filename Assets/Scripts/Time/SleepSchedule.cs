using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepSchedule : MonoBehaviour
{
    [SerializeField, Range(0.0f, 24.0f)] private float _MorningTime = 6.0f; // morning time in 00:00-24:00 range
    [SerializeField, Range(0.0f, 24.0f)] private float _NightTime = 22.0f;
    [Space(5)]
    [SerializeField, Min(0)] private float _SleepTime = 1.0f;
    [SerializeField] private GameObject _Player;
    [SerializeField] private FloraMaster _FloraMaster;

    private DateTime _DateTime;

    private PlayerMovement _PlayerMovement;
    private InteractableArea _InteractableArea;

    private bool _Sleep = false;
    private float _TotalTimeToSleep = 0;

    public float MorningTime => _MorningTime;
    public float NightTime => _NightTime;

    private void Awake()
    {
        if (_Player == null)
            Debug.LogError("need reference to current Player in scene");
        if (_FloraMaster == null)
            Debug.LogError("need reference to current FloraMaster in scene");

        _DateTime = GetComponent<DateTime>();
        _PlayerMovement = _Player.GetComponent<PlayerMovement>();
        _InteractableArea = _Player.GetComponent<InteractableArea>();
    }

    private void Start()
    {
        _TotalTimeToSleep = (24.0f + (_MorningTime - _NightTime)) % 24.0f;
    }

    private void Update()
    {
        if (!_Sleep && _DateTime.HourClock + (_DateTime.MinuteClock / 60) >= _NightTime)
        {
            _Sleep = true;
            StartCoroutine(Sleep());
        }
    }

    private IEnumerator Sleep()
    {
        // sleep

        _PlayerMovement.enabled = false;
        _InteractableArea.enabled = false;

        yield return new WaitForSeconds(_SleepTime);

        // wake up

        _PlayerMovement.enabled = true;
        _InteractableArea.enabled = true;

        _FloraMaster.Notify();

        _DateTime.AccelerateTime(_NightTime, _TotalTimeToSleep);

        _Sleep = false;
    }
}
