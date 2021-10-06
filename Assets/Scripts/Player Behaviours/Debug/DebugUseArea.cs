using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

public class DebugUseArea : MonoBehaviour, IUsable, IDestructable
{
    public float Strength { get; set; } = DurabilityStrengths.UNARMED;
    public float Durability { get; set; } = 10;

    public IList<string> Filter(ItemLabels labels) => new List<string> { labels.CROP };

    public void OnUse(UseContext context)
    {
        if (context.used is IDestructor)
        {
            
        }
    }
}
