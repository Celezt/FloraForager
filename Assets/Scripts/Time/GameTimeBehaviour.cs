using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyBox;
using IngameDebugConsole;

public class GameTimeBehaviour : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(UpdateTimeLoop());
        DebugLogConsole.AddCommandInstance("time.skip", "Accelerates time from point in current day by given amount in hours", nameof(GameTime.Instance.AccelerateTime), GameTime.Instance);
    }

    private void Update()
    {
        GameTime.Instance.ElapsedTime += (decimal)Time.deltaTime;
    }

    private IEnumerator UpdateTimeLoop()
    {
        while (true)
        {
            GameTime.Instance.UpdateTime();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
