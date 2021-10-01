using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DebugUseArea : MonoBehaviour, IUsable
{
    public int Priority => 0;

    public void OnUse(UseContext context)
    {
        Debug.Log("slam");
    }
}
