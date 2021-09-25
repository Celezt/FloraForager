using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunControl : MonoBehaviour
{
    [SerializeField] private GameObject _Sun;
    [SerializeField, Range(0.0f, 360.0f)] private float _Yaw = 30.0f;
    [SerializeField, Range(1.0f, 10.0f)] private float _Damping = 4.5f;

    private SleepSchedule _SleepSchedule;

    private float _Pitch;
    private float _NewPitch;
    private float _MorningOffset;

    private void Awake()
    {
        if (_Sun == null)
            this.enabled = false;

        _SleepSchedule = GetComponent<SleepSchedule>();
    }

    private void Start()
    {
        _MorningOffset = (360.0f / 24.0f) * _SleepSchedule.MorningTime;
    }

    private void Update()
    {
        _Pitch = 360.0f * (GameTime.Instance.CurrentTime / 24.0f) - _MorningOffset;

        float angleDiff = Mathf.Abs(_NewPitch) - Mathf.Abs(_Pitch);
        _NewPitch = (angleDiff < 10.0f) ? Mathf.SmoothStep(_NewPitch, _Pitch, Time.deltaTime * _Damping) : _Pitch;

        _Sun.transform.rotation = Quaternion.Euler(_NewPitch, _Yaw, 0.0f);
    }
}
