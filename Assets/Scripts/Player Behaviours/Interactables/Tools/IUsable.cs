using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable : IPlayerInteractable
{
    public void OnUse(UseContext context);
}
