using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Time;

[System.Serializable]
public struct SpeedEffect : IEffect
{
    public Duration Duration { get; set; }

    public float SpeedMultiplier;
    public float Time;

    void IEffect.OnEffect(UseContext context)
    {
        Duration = context.transform.GetComponent<PlayerMovement>().AddSpeedMultiplier(SpeedMultiplier, Time);
    }
}
