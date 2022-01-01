using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;
using IngameDebugConsole;

public class GameTimeBehaviour : MonoBehaviour
{
    public static bool Active = true;

    private void Awake()
    {
        GameTime.Instance.UpdateTime();
        DebugLogConsole.AddCommandInstance("time.skip", "Accelerates time from point in current day by given amount in hours", nameof(CommandAccelerateTime), this);
    }

    private void Update()
    {
        GameTime.Instance.ElapsedTime += Active ? (decimal)Time.deltaTime : 0M;
        GameTime.Instance.UpdateTime();
    }

    public void CommandAccelerateTime(float hours) => GameTime.Instance.AccelerateTime(hours);
}
