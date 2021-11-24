using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "GameTime", menuName = "Game Logic/GameTime")]
[System.Serializable]
public class GameTime : SerializedScriptableSingleton<GameTime>, IStreamer, IStreamable<GameTime.Data>
{
    [OdinSerialize]
    private System.Guid _Guid;

    [Space(5)]
    [SerializeField, Min(0), LabelText("Seconds Per In-Game Hour")] 
    private float _InGameHour = 60.0f; // how long an in-game hour lasts in seconds

    [Space(5)]
    [SerializeField, Min(0)] private int _CurrentYear;
    [SerializeField, Min(0)] private int _CurrentMonth;
    [SerializeField, Min(0)] private int _CurrentDay;
    [SerializeField, Min(0)] private int _CurrentHour = 6;

    [Space(5)]
    [SerializeField, Min(0)] private int _HoursPerDay = 24;
    [SerializeField, Min(0)] private int _DaysPerMonth = 30;
    [SerializeField, Min(0)] private int _MonthsPerYear = 12;

    [Space(5)]
    [SerializeField]
    private float _ClockUpdateFrequency = 5.0f;

    [Space(5)]
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private readonly string[] Weekdays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    [System.NonSerialized]
    private Data _Data = new Data();

    public class Data
    {
        public decimal ElapsedTime;
    }

    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }
    public void OnBeforeSaving() { }

    private float _MinuteClock = 0.0f;
    private float _HourClock = 0.0f;

    private int _DayCalendar = 0;
    private int _MonthCalendar = 0;

    public float InGameHour => _InGameHour;

    /// <summary>
    /// current time in day in common format 0.0-24.0
    /// </summary>
    public float CurrentTime => HourClock +  (MinuteClock / 60.0f);

    public float HourClock => _HourClock;
    public float MinuteClock => _MinuteClock;

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
        "<mspace=0.75em>{0:00}</mspace>" +
        "<mspace=0.30em>:</mspace>" +
        "<mspace=0.75em>{1:00}</mspace>", Mathf.Floor(_HourClock), Mathf.Floor(Mathf.Floor(_MinuteClock) / _ClockUpdateFrequency) * _ClockUpdateFrequency);
    public string Date => string.Format("{0:0000}/{1:00}/{2:00}", 
        Year, _MonthCalendar, _DayCalendar);
    public string Weekday => string.Format("{0} {1:00}/{2:00}", 
        Weekdays[(_DayCalendar - 1) % Weekdays.Length].Substring(0, 3),
        _DayCalendar, _MonthCalendar);

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        GameManager.AddStreamer(Instance);
    }

    public void UpdateTime()
    {
        Hour = _CurrentHour + ((float)_Data.ElapsedTime / _InGameHour);

        Day = _CurrentDay + (Mathf.FloorToInt(Hour) / _HoursPerDay);
        Month = _CurrentMonth + (Day / _DaysPerMonth);
        Year = _CurrentYear + (Month / _MonthsPerYear);

        _MinuteClock = (float)_Data.ElapsedTime * (60.0f / _InGameHour) % 60.0f; // keep to common standards of 00:00-24:00
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

        ElapsedTime = (decimal)acceleratedTime;

        UpdateTime();
    }

    public void UpLoad()
    {
        GameManager.Stream.Load(_Guid, OnUpload());
    }
    public void Load()
    {
        _Data = new Data();

        if (!GameManager.Stream.TryGet(_Guid, out object value))
            return;

        OnLoad(value);
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();

        OnBeforeSaving();
    }
}
