using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using IngameDebugConsole;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField]
    private AnimationClip _passOutClip;
    [SerializeField]
    private AnimationClip _sleepClip;
    [SerializeField]
    private float _MaxStamina = 100.0f;
    [SerializeField]
    private float _WalkingDrain = 0.1f;  // amount stamina drained every second
    [SerializeField]
    private float _RunningDrain = 0.2f;
    [SerializeField]
    private float _NightDrain = 7.5f;
    [SerializeField, Range(0.0f, 1.0f)]
    private float _PassOutPenalty = 0.25f; // percentage of stamina lost

    public event System.Action OnStaminaDrained = delegate { };
    public event System.Action OnStaminaEmptied = delegate { };

    private PlayerMovement _PlayerMovement;
    private AnimationBehaviour _animationBehaviour;

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

    public void ChangeStamina(float change) => Stamina += change;
    public void SetStamina(float value) => Stamina = value;

    private void Awake()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        _animationBehaviour = GetComponentInChildren<AnimationBehaviour>();

        DebugLogConsole.AddCommandInstance("player.stamina_change", "Change player's stamina", nameof(ChangeStamina), this);
        DebugLogConsole.AddCommandInstance("player.stamina_set", "Set player's stamina", nameof(SetStamina), this);
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

        OnStaminaEmptied += PassOut;
    }

    private void Update()
    {
        OnStaminaDrained.Invoke();
        if (_Stamina <= 0.0f && !SleepSchedule.Instance.IsSleeping)
        {
            OnStaminaEmptied.Invoke();
        }
    }

    public void Drain(float staminaDrain)
    {
        _Stamina -= staminaDrain * Time.deltaTime;
    }

    private void WalkingDrain()
    {
        Drain(_WalkingDrain * ((_PlayerMovement.RawVelocity.magnitude > float.Epsilon) ? 1.0f : 0.0f));
    }
    private void RunningDrain()
    {
        Drain(_RunningDrain * ((_PlayerMovement.RawVelocity.magnitude > float.Epsilon) ? 1.0f : 0.0f));
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

    private void PassOut()
    {
        SleepSchedule.Instance.StartSleeping(true);
        StartCoroutine(PassOutAnimation());
    }
    private IEnumerator PassOutAnimation()
    {
        _animationBehaviour.CustomMotionRaise(_passOutClip);
        yield return new WaitForSeconds(_passOutClip.length);
        _animationBehaviour.CustomMotionRaise(_sleepClip, loop: true);
        yield return new WaitUntil(() => !SleepSchedule.Instance.IsSleeping);
        _animationBehaviour.ForceCancelCustomMotion();
    }

    public void Recover(bool penalty)
    {
        _Stamina = penalty ? _MaxStamina * (1.0f - _PassOutPenalty) : _MaxStamina;
    }
}
