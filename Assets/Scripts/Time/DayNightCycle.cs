using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] 
    private Light _Sun;
    [SerializeField, Range(0.0f, 360.0f)] 
    private float _Yaw = 30.0f;
    [SerializeField] 
    private Gradient _SunColor;
    [SerializeField] 
    private AnimationCurve _SunIntensity;

    private Quaternion _CurrentRotation;
    private Quaternion _NewRotation;
    private float _MorningOffset;
    private float _Damping;

    private void Awake()
    {
        RenderSettings.sun = _Sun;
    }

    private void Start()
    {
        _MorningOffset = (360.0f / 24.0f) * SleepSchedule.Instance.MorningTime;
    }

    private void LateUpdate()
    {
        float time = (GameTime.Instance.CurrentTime / 24.0f);

        _Sun.color = _SunColor.Evaluate(time);
        _Sun.intensity = _SunIntensity.Evaluate(time);

        float pitch = 360.0f * time - _MorningOffset;
        _CurrentRotation = Quaternion.Euler(pitch, _Yaw, 0.0f);

        float angleDiff = _Damping = Quaternion.Angle(_NewRotation, _CurrentRotation);
        _NewRotation = (angleDiff < 2.5f) ? Quaternion.Lerp(_NewRotation, _CurrentRotation, _Damping * Time.deltaTime) : _CurrentRotation;

        _Sun.transform.localRotation = _NewRotation;
    }
}
