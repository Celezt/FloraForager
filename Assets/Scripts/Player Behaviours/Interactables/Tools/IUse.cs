using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUse
{
    public void OnUse(UseContext context);
    public void OnUpdate(UseContext context);
    public void OnInactive(UseContext context);
    public void OnActive(UseContext context);
}
