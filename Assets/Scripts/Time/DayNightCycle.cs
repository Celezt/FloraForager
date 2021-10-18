using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light _Sun;
    [SerializeField, Range(0.0f, 360.0f)] private float _Yaw = 30.0f;
    [SerializeField, Range(1.0f, 10.0f)] private float _Damping = 4.5f;
    [SerializeField] private Gradient _SunColor;
    [SerializeField] private AnimationCurve _SunIntensity;

    private float _Pitch;
    private float _NewPitch;
    private float _MorningOffset;

    private void Awake()
    {
        if (_Sun == null)
            this.enabled = false;

        if (RenderSettings.sun != null)
            _Sun = RenderSettings.sun;
    }

    private void Start()
    {
        _MorningOffset = (360.0f / 24.0f) * SleepSchedule.Instance.MorningTime;
    }

    private void Update()
    {
        float time = (GameTime.Instance.CurrentTime / 24.0f);

        _Sun.color = _SunColor.Evaluate(time);
        _Sun.intensity = _SunIntensity.Evaluate(time);

        _Pitch = 360.0f * (GameTime.Instance.CurrentTime / 24.0f) - _MorningOffset;

        float angleDiff = Mathf.Abs(_NewPitch) - Mathf.Abs(_Pitch);
        _NewPitch = (angleDiff < 5.0f) ? Mathf.SmoothStep(_NewPitch, _Pitch, _Damping * Time.deltaTime) : _Pitch;

        _Sun.transform.localRotation = Quaternion.Euler(_NewPitch, _Yaw, 0.0f);
    }
}
