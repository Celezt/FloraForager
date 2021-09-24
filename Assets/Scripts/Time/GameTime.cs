using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameTime : MonoBehaviour
{
    [Space(5), Header("Game")]
    [SerializeField, Min(0)] private float _InGameHour = 60.0f; // how long an in-game hour lasts in seconds

    [Space(5)]
    [SerializeField, Min(0)] private int _CurrentYear = 0; 
    [SerializeField, Min(0)] private int _CurrentMonth = 0;
    [SerializeField, Min(0)] private int _CurrentDay = 0;
    [SerializeField, Min(0)] private int _CurrentHour = 0;

    [Space(5), Header("Calendar")]
    [SerializeField, Min(0)] private int _HoursPerDay = 24; 
    [SerializeField, Min(0)] private int _DaysPerMonth = 30;
    [SerializeField, Min(0)] private int _MonthsPerYear = 12;

    private decimal _ElapsedTime = 0.0M;

    private float _MinuteClock = 0.0f;
    private float _HourClock = 0.0f;

    private int _DayCalendar = 0;
    private int _MonthCalendar = 0;

    public decimal ElapsedTime => _ElapsedTime;
    public float InGameHour => _InGameHour;

    /// <summary>
    /// game time in common format 0.0-24.0
    /// </summary>
    public float CurrentTime => HourClock + (_MinuteClock / 60.0f);

    public float MinuteClock => _MinuteClock;
    public float HourClock => _HourClock;

    public float Hour { get; private set; }
    public int Day { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public string DigitalTime => string.Format("{0:00}:{1:00}", _HourClock, _MinuteClock);
    public string Date => string.Format("{0:0000}/{1:00}/{2:00}", Year, _MonthCalendar, _DayCalendar);

    private void Start()
    {
        StartCoroutine(UpdateTimeLoop());
    }

    private void Update()
    {
        _ElapsedTime += (decimal)Time.deltaTime;
    }
    
    public IEnumerator UpdateTimeLoop()
    {
        while (true)
        {
            UpdateTime();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void UpdateTime()
    {
        Hour = _CurrentHour + ((float)_ElapsedTime / _InGameHour);

        Day = _CurrentDay + (Mathf.FloorToInt(Hour) / _HoursPerDay);
        Month = _CurrentMonth + (Day / _DaysPerMonth);
        Year = _CurrentYear + (Month / _MonthsPerYear);

        _MinuteClock = (float)_ElapsedTime * (60.0f / _InGameHour) % 60.0f; // keep to common standards of 00:00-24:00
        _HourClock = (int)Hour * (24.0f / _HoursPerDay) % 24.0f;

        _DayCalendar = 1 + Day % _DaysPerMonth;
        _MonthCalendar = 1 + Month % _MonthsPerYear;
    }

    /// <summary>
    /// accelerates time from point in current day by given amount in hours
    /// </summary>
    public void AccelerateTime(float from, float hours)
    {
        float elapsedTimeToDay = (int)(Hour / _HoursPerDay) * _HoursPerDay * InGameHour; // total time elapsed to this day
        float acceleratedTime = (elapsedTimeToDay + 
            (from * InGameHour) * (_HoursPerDay / 24.0f) + 
            (hours * InGameHour) * (_HoursPerDay / 24.0f) - 
            (_CurrentHour * _InGameHour)); // time to accelerate by

        SetElapsedTime((decimal)acceleratedTime);
        UpdateTime();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * 3.00f, Date);
        Handles.Label(transform.position + Vector3.up * 2.60f, DigitalTime);
    }
#endif

    public void SetElapsedTime(decimal elapsedTime)
    {
        _ElapsedTime = elapsedTime;
    }
}
