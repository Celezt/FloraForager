using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private GameObject _StaminaSlider;
    [SerializeField] private float _MaxStamina = 100.0f;
    [SerializeField] private float _StmMovementDrain = 0.04f;  // amount stamina drained every second
    [SerializeField] private float _StmActionDrain = 0.05f;
    [SerializeField] private float _StmNightDrain = 7.5f;

    public UnityEvent OnStaminaDrained;

    private PlayerMovement _PlayerMovement;
    private Slider _Slider;

    private float _Stamina;
    private float _Sleepy; // rate to increase stamina drain rate

    public float Stamina => _Stamina;

    private void Awake()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        _Slider = _StaminaSlider.GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        _Stamina = _MaxStamina;

        OnStaminaDrained.AddListener(MovementDrain);
        OnStaminaDrained.AddListener(NightDrain);
    }

    private void Update()
    {
        OnStaminaDrained.Invoke();
        if (_Stamina <= 0.0f && !SleepSchedule.Instance.IsSleeping)
        {
            SleepSchedule.Instance.StartSleeping(this, GameTime.Instance.CurrentTime);
        }
    }

    private void LateUpdate()
    {
        _Slider.value = (_Stamina / _MaxStamina);
    }

    private void MovementDrain()
    {
        float playerSpeed = (_PlayerMovement.RawVelocity.magnitude / (_PlayerMovement.Speed * Time.fixedDeltaTime));
        Drain(_StmMovementDrain * playerSpeed);
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
