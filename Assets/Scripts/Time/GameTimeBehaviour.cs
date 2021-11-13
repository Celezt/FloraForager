using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;
using IngameDebugConsole;

public class GameTimeBehaviour : MonoBehaviour
{
    private void Awake()
    {
        GameTime.Instance.UpdateTime();
        DebugLogConsole.AddCommandInstance("time.skip", "Accelerates time from point in current day by given amount in hours", nameof(GameTime.Instance.AccelerateTime), GameTime.Instance);
    }

    private void Update()
    {
        GameTime.Instance.ElapsedTime += (decimal)Time.deltaTime;
        GameTime.Instance.UpdateTime();
    }
}
