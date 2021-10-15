using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Time;

public interface IEffect
{
    public Duration Duration { get; set; }
    public void OnEffect(UseContext context);
}
