using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DayNightCycle : MonoBehaviour
{
    [Title("Sun")]
    [SerializeField] 
    private Light _Sun;
    [SerializeField]
    private Gradient _SunColor;
    [SerializeField]
    private AnimationCurve _SunIntensity;

    [Title("Fog")]
    [SerializeField]
    private bool _FogEnabled = true;
    [SerializeField]
    private Color _FogColor;

    [Title("Other")]
    [SerializeField, Range(0.0f, 360.0f)] 
    private float _Yaw = 30.0f;
    [SerializeField]
    private Gradient _AmbientColor;
    [SerializeField, Tooltip("Editor Tool")]
    private float _TimeOfDay;

    private Quaternion _CurrentRotation;
    private float _MorningOffset;

    private float _EditorTime;

    private void Awake()
    {
        RenderSettings.sun = _Sun;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.fog = _FogEnabled;
        RenderSettings.fogColor = _FogColor;
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

        _Sun.transform.localRotation = _CurrentRotation;

        RenderSettings.ambientLight = _AmbientColor.Evaluate(time);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_Sun == null || _EditorTime == _TimeOfDay)
            return;

        _EditorTime = _TimeOfDay;

        RenderSettings.sun = _Sun;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        RenderSettings.fog = _FogEnabled;
        RenderSettings.fogColor = _FogColor;

        _MorningOffset = (360.0f / 24.0f) * 6.0f;

        float time = (_TimeOfDay = (_TimeOfDay % 24.0f + 24.0f) % 24.0f) / 24.0f;

        _Sun.color = _SunColor.Evaluate(time);
        _Sun.intensity = _SunIntensity.Evaluate(time);

        float pitch = 360.0f * time - _MorningOffset;
        _CurrentRotation = Quaternion.Euler(pitch, _Yaw, 0.0f);

        _Sun.transform.localRotation = _CurrentRotation;

        RenderSettings.ambientLight = _AmbientColor.Evaluate(time);
    }
#endif
}
