using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SleepSchedule : MonoBehaviour
{
    [SerializeField, Range(0.0f, 24.0f)] private float _MorningTime = 6.0f; // morning time in 00:00-24:00 range
    [SerializeField, Range(0.0f, 24.0f)] private float _NightTime = 22.0f;
    [Space(5)]
    [SerializeField, Min(0)] private float _SleepTime = 1.0f;
    [SerializeField] private GameObject _Player;
    [SerializeField] private FloraMaster _FloraMaster;

    private GameTime _GameTime;

    private PlayerMovement _PlayerMovement;
    private InteractableArea _InteractableArea;
    private Rigidbody _PlayerRigidbody;

    private bool _Sleeping = false;      // if the player is currently sleeping
    private float _TotalTimeToSleep = 0; // total time in hours for player to sleep

    public float MorningTime => _MorningTime;
    public float NightTime => _NightTime;

    public bool SleepNow { get; set; } = false;

    private void Awake()
    {
        if (_Player == null)
            Debug.LogError("need reference to current Player in scene");
        if (_FloraMaster == null)
            Debug.LogError("need reference to current FloraMaster in scene");

        _GameTime = GetComponent<GameTime>();
        _PlayerMovement = _Player.GetComponent<PlayerMovement>();
        _InteractableArea = _Player.GetComponent<InteractableArea>();
        _PlayerRigidbody = _Player.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _TotalTimeToSleep = (24.0f + (_MorningTime - _NightTime)) % 24.0f;
    }

    private void Update()
    {
        if (SleepNow || (!_Sleeping && _GameTime.CurrentTime >= _NightTime))
        {
            _Sleeping = true;
            SleepNow = false;

            StartCoroutine(Sleep());
        }
    }

    private IEnumerator Sleep()
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

        _FloraMaster.Notify();

        _GameTime.AccelerateTime(_NightTime, _TotalTimeToSleep);

        _Sleeping = false;
    }
}
