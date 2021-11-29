using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using IngameDebugConsole;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] 
    private float _MaxStamina = 100.0f;
    [SerializeField] 
    private float _WalkingDrain = 0.04f;  // amount stamina drained every second
    [SerializeField] 
    private float _RunningDrain = 0.09f;
    [SerializeField] 
    private float _NightDrain = 7.5f;
    [SerializeField, Range(0.0f, 1.0f)] 
    private float _PassOutPenalty = 0.25f; // percentage of stamina lost

    public event System.Action OnStaminaDrained = delegate { };
    public event System.Action OnStaminaEmptied = delegate { };

    private PlayerMovement _PlayerMovement;

    private float _Stamina;
    private float _Sleepy; // rate to increase stamina drain rate

    public float MaxStamina => _MaxStamina;
    public float Stamina
    {
        get => _Stamina;
        set
        {
            _Stamina = Mathf.Clamp(value, 0.0f, _MaxStamina);
        }
    }

    private void Awake()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        DebugLogConsole.AddCommandInstance("player.stamina_change", "Change player's stamina", nameof(ChangeStamina), this);
    }

    private void Start()
    {
        OnStaminaDrained += NightDrain;
        OnStaminaDrained += WalkingDrain;

        _PlayerMovement.OnPlayerRunCallback += (speed, isRunning) =>
        {
            if (!isRunning)
            {
                OnStaminaDrained -= RunningDrain;
                OnStaminaDrained += WalkingDrain;
            }
            else
            {
                OnStaminaDrained -= WalkingDrain;
                OnStaminaDrained += RunningDrain;
            }
        };
    }

    private void Update()
    {
        OnStaminaDrained.Invoke();
        if (_Stamina <= 0.0f && !SleepSchedule.Instance.IsSleeping)
        {
            OnStaminaEmptied.Invoke();
            SleepSchedule.Instance.StartSleeping(true);
        }
    }

    private void WalkingDrain()
    {
        Drain(_WalkingDrain * ((_PlayerMovement.Velocity.magnitude > 0.01f) ? 1.0f : 0.0f));
    }

    private void RunningDrain()
    {
        Drain(_RunningDrain * ((_PlayerMovement.Velocity.magnitude > 0.01f) ? 1.0f : 0.0f));
    }

    private void NightDrain()
    {
        if (SleepSchedule.Instance.IsNightTime)
        {
            float timeToMorning = SleepSchedule.Instance.TimeToMorning(GameTime.Instance.CurrentTime);
            float totalTime = SleepSchedule.Instance.NightToMorning;

            float sleepy = ((SleepSchedule.Instance.TimeToNight(SleepSchedule.Instance.MorningTime) - timeToMorning) - totalTime) / totalTime;
            Drain(_NightDrain * sleepy);
        }
    }

    public void ChangeStamina(float change) => Stamina = change;

    public void Drain(float staminaDrain)
    {
        _Stamina -= staminaDrain * Time.deltaTime;
    }

    public void Recover(bool penalty)
    {
        _Stamina = (penalty) ? _MaxStamina * (1.0f - _PassOutPenalty) : _MaxStamina;
    }
}
