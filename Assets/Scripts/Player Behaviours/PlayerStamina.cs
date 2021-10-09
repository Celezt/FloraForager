using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private GameObject _StaminaSlider;
    [SerializeField] private float _MaxStamina = 100.0f;
    [SerializeField] private float _StmWalkingDrain = 0.04f;  // amount stamina drained every second
    [SerializeField] private float _StmRunningDrain = 0.09f;
    [SerializeField] private float _StmActionDrain = 0.07f;
    [SerializeField] private float _StmNightDrain = 7.5f;

    public UnityEvent OnStaminaDrained;

    private PlayerMovement _PlayerMovement;
    private Slider _Slider;

    private float _Stamina;
    private float _Sleepy; // rate to increase stamina drain rate

    public float Stamina
    {
        get => _Stamina;
        set
        {
            _Stamina = Mathf.Clamp(_Stamina + value, 0.0f, _MaxStamina);
        }
    }

    private void Awake()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        _Slider = _StaminaSlider.GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        _Stamina = _MaxStamina;

        OnStaminaDrained.AddListener(NightDrain);
        OnStaminaDrained.AddListener(WalkingDrain);

        _PlayerMovement.OnPlayerRunCallback += (speed, isRunning) =>
        {
            if (!isRunning)
            {
                OnStaminaDrained.RemoveListener(RunningDrain);
                OnStaminaDrained.AddListener(WalkingDrain);
            }
            else
            {
                OnStaminaDrained.AddListener(RunningDrain);
                OnStaminaDrained.RemoveListener(WalkingDrain);
            }
        };
    }

    private void Update()
    {
        OnStaminaDrained.Invoke();
        if (_Stamina <= 0.0f && !SleepSchedule.Instance.IsSleeping)
        {
            SleepSchedule.Instance.StartSleeping();
        }
    }

    private void LateUpdate()
    {
        _Slider.value = (_Stamina / _MaxStamina);
    }

    private void WalkingDrain()
    {
        Drain(_StmWalkingDrain * ((_PlayerMovement.Velocity.magnitude > float.Epsilon) ? 1.0f : 0.0f));
    }

    private void RunningDrain()
    {
        Drain(_StmRunningDrain * ((_PlayerMovement.Velocity.magnitude > float.Epsilon) ? 1.0f : 0.0f));
    }

    private void NightDrain()
    {
        float currentTime = GameTime.Instance.CurrentTime;

        float timeToNight = SleepSchedule.Instance.TimeToNight(currentTime);
        float timeToMorning = SleepSchedule.Instance.TimeToMorning(currentTime);

        float totalTime = SleepSchedule.Instance.NightToMorning;

        bool isNightTime = ((currentTime != SleepSchedule.Instance.MorningTime) && timeToNight > timeToMorning);

        if (isNightTime)
        {
            float sleepy = ((SleepSchedule.Instance.TimeToNight(SleepSchedule.Instance.MorningTime) - timeToMorning) - totalTime) / totalTime;
            Drain(_StmNightDrain * sleepy);
        }
    }

    public void Drain(float staminaDrain)
    {
        _Stamina -= staminaDrain * Time.deltaTime;
    }

    public void Recover()
    {
        _Stamina = _MaxStamina;
    }
}
