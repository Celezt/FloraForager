using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DebugUseArea : MonoBehaviour, IUsable, IDestructable
{
    public float Strength { get; set; } = DurabilityStrengths.UNARMED;
    public float Durability { get; set; } = 10;

    public void OnUse(UseContext context)
    {
        if (context.used is IDestructor)
        {
            
        }
    }
}
