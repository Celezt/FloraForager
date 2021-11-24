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
    private Gradient _AmbientColor;
    [SerializeField] 
    private AnimationCurve _SunIntensity;

    private Quaternion _CurrentRotation;
    private float _MorningOffset;

    private void Awake()
    {
        RenderSettings.sun = _Sun;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    }

    private void Start()
    {
        _MorningOffset = (360.0f / 24.0f) * SleepSchedule.Instance.MorningTime;
    }

    private void LateUpdate()
    {
        float time = (GameTime.Instance.CurrentTime / 24.0f);

        RenderSettings.ambientLight = _AmbientColor.Evaluate(time);

        _Sun.color = _SunColor.Evaluate(time);
        _Sun.intensity = _SunIntensity.Evaluate(time);

        float pitch = 360.0f * time - _MorningOffset;
        _CurrentRotation = Quaternion.Euler(pitch, _Yaw, 0.0f);

        _Sun.transform.localRotation = _CurrentRotation;
    }
}
