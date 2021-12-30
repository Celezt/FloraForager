using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Time;

[System.Serializable]
public struct SpeedEffect : IEffect
{
    public float SpeedMultiplier;
    public float Time;

    private Duration _duration;

    void IEffect.OnEffect(UseContext context)
    {
        if (_duration.IsActive)
            context.transform.GetComponent<PlayerMovement>().SpeedMultipliers.Remove(_duration);

        _duration = context.transform.GetComponent<PlayerMovement>().SpeedMultipliers.Add(Time, SpeedMultiplier);
    }
}
