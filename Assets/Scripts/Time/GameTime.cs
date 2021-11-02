using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "GameTime", menuName = "Game Logic/GameTime")]
[System.Serializable]
public class GameTime : SerializedScriptableSingleton<GameTime>, IStreamer, IStreamable<GameTime.Data>
{
    [Space(5), Header("Game")]
    [OdinSerialize, Min(0)] private float _InGameHour = 60.0f; // how long an in-game hour lasts in seconds

    [Space(5)]
    [OdinSerialize, Min(0)] private int _CurrentYear; 
    [OdinSerialize, Min(0)] private int _CurrentMonth;
    [OdinSerialize, Min(0)] private int _CurrentDay;
    [OdinSerialize, Min(0)] private int _CurrentHour = 6;

    [Space(5), Header("Calendar")]
    [OdinSerialize, Min(0)] private int _HoursPerDay = 24; 
    [OdinSerialize, Min(0)] private int _DaysPerMonth = 30;
    [OdinSerialize, Min(0)] private int _MonthsPerYear = 12;

    [Space(5), Header("Misc")]
    [OdinSerialize]
    private System.Guid _Guid;

    [System.NonSerialized]
    private Data _Data = new Data();

    public class Data
    {
        public decimal ElapsedTime = 0.0M;
    }

    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }
    public void OnBeforeSaving() { }

    public void UpLoad()
    {
        GameManager.Stream.Load(_Guid, OnUpload());
    }
    public void Load()
    {
        OnLoad(GameManager.Stream.Get(_Guid));
    }
    public void BeforeSaving() { }

    private float _MinuteClock = 0.0f;
    private float _HourClock = 0.0f;

    private int _DayCalendar = 0;
    private int _MonthCalendar = 0;

    public float InGameHour => _InGameHour;

    /// <summary>
    /// game time in common format 0.0-24.0
    /// </summary>
    public float CurrentTime => HourClock + (_MinuteClock / 60.0f);

    public float MinuteClock => _MinuteClock;
    public float HourClock => _HourClock;

    public decimal ElapsedTime
    {
        get => _Data.ElapsedTime;
        set => _Data.ElapsedTime = value;
    }

    public float Hour { get; private set; }
    public int Day { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public string DigitalTime => string.Format(
        "<mspace=0.60em>{0:00}</mspace>" +
        "<mspace=0.30em>:</mspace>" +
        "<mspace=0.60em>{1:00}</mspace>", _HourClock, _MinuteClock);
    public string Date => string.Format("{0:0000}/{1:00}/{2:00}", Year, _MonthCalendar, _DayCalendar);

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();

        GameManager.AddStreamer(this);
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void PlayModeInitialize()
    {
        GameManager.AddStreamer(Instance);
    }
#endif

    public void UpdateTime()
    {
        Hour = _CurrentHour + ((float)_Data.ElapsedTime / _InGameHour);

        Day = _CurrentDay + (Mathf.FloorToInt(Hour) / _HoursPerDay);
        Month = _CurrentMonth + (Day / _DaysPerMonth);
        Year = _CurrentYear + (Month / _MonthsPerYear);

        _MinuteClock = (float)_Data.ElapsedTime * (60.0f / _InGameHour) % 60.0f; // keep to common standards of 00:00-24:00
        _HourClock = (int)Hour * (24.0f / _HoursPerDay) % 24.0f;

        _MinuteClock = Mathf.Floor(_MinuteClock);
        _HourClock = Mathf.Floor(_HourClock);

        _DayCalendar = 1 + Day % _DaysPerMonth;
        _MonthCalendar = 1 + Month % _MonthsPerYear;

        GameTimeUI.Instance.UpdateText(Date, DigitalTime);
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

        _Data.ElapsedTime = (decimal)acceleratedTime;

        UpdateTime();
    }
}
