using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Time;
using System;

public class DurationCollection<T> : IEnumerable<KeyValuePair<Duration, T>>, IEnumerable
{
    Dictionary<Duration, T> _durations = new Dictionary<Duration, T>();

    Stopwatch _stopwatch;
    float _oldTime;

    public T this[Duration key] => _durations[key];

    public int Count
    {
        get
        {
            if (!IsUpdated)
                Update();

            return _durations.Count;
        }
    }

    public IReadOnlyCollection<T> Values
    {
        get
        {
            if (!IsUpdated)
                Update();

            return _durations.Values;
        }
    }
    public IReadOnlyCollection<Duration> Keys
    {
        get
        {
            if (!IsUpdated)
                Update();

            return _durations.Keys;
        }
    }
    public IReadOnlyDictionary<Duration, T> Durations
    {
        get
        {
            if (!IsUpdated)
                Update();

            return _durations;
        }
    }

    /// <summary>
    /// If it has been updated this frame.
    /// </summary>
    public bool IsUpdated
    {
        get
        {
            bool value = false;

            if (_stopwatch.Timer == 0)
                _stopwatch = Stopwatch.Initialize();
            value = _stopwatch.Timer == _oldTime;

            _oldTime = _stopwatch.Timer;

            return value;
        }
    }

    /// <summary>
    /// Add value that needs to be manually removed.
    /// </summary>
    /// <returns>Identifier.</returns>
    public Duration Add(T value) => Add(Duration.Infinity, value);
    /// <summary>
    /// Add value that only lasts for the duration.
    /// </summary>
    /// <returns>Identifier.</returns>
    public Duration Add(float duration, T value) => Add(new Duration(duration), value);
    /// <summary>
    /// Add value that only lasts for the duration.
    /// </summary>
    /// <returns>Identifier.</returns>
    public Duration Add(Duration duration, T value)
    {
        _durations.Add(duration, value);

        return duration;
    }

    public bool Remove(Duration identifier) => _durations.Remove(identifier);

    public void Clear() => _durations.Clear();

    /// <summary>
    /// Manually update.
    /// </summary>
    public void Update()
    {
        List<Duration> removals = new List<Duration>();
        foreach (Duration duration in _durations.Keys)
            if (!duration.IsActive)
                removals.Add(duration);

        foreach (Duration duration in removals)
            _durations.Remove(duration);
    }

    public IEnumerator<KeyValuePair<Duration, T>> GetEnumerator()
    {
        if (!IsUpdated)
            Update();

        return _durations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        if (!IsUpdated)
            Update();

        return _durations.GetEnumerator();
    }
}

public class DurationCollection : IEnumerable<Duration>, IEnumerable
{
    HashSet<Duration> _durations = new HashSet<Duration>();

    Stopwatch _stopwatch;
    float _oldTime;

    public int Count
    {
        get
        {
            if (!IsUpdated)
                Update();

            return _durations.Count;
        }
    }

    public IReadOnlyCollection<Duration> Durations
    {
        get
        {
            if (!IsUpdated)
                Update();

            return _durations;
        }
    }

    /// <summary>
    /// If it has been updated this frame.
    /// </summary>
    public bool IsUpdated
    {
        get
        {
            bool value = false;

            if (_stopwatch.Timer == 0)
                _stopwatch = Stopwatch.Initialize();
            value = _stopwatch.Timer == _oldTime;

            _oldTime = _stopwatch.Timer;

            return value;
        }
    }

    /// <summary>
    /// Add duration that expires.
    /// </summary>
    /// <returns>Identifier.</returns>
    public Duration Add(float duration) => Add(new Duration(duration));
    /// <summary>
    /// Add duration that expires.
    /// </summary>
    /// <returns>Identifier.</returns>
    public Duration Add(Duration duration)
    {
        _durations.Add(duration);

        return duration;
    }

    public bool Remove(Duration identifier) => _durations.Remove(identifier);

    public void Clear() => _durations.Clear();

    /// <summary>
    /// Manually update.
    /// </summary>
    public void Update() => _durations.RemoveWhere(x => !x.IsActive);

    public IEnumerator<Duration> GetEnumerator()
    {
        if (!IsUpdated)
            Update();

        return _durations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        if (!IsUpdated)
            Update();

        return _durations.GetEnumerator();
    }
}
