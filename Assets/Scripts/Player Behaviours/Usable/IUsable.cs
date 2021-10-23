using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable : IPlayerInteractable
{
    public void OnUse(UsedContext context);

    /// <summary>
    /// Whitelist users with label.
    /// </summary>
    /// <returns>To whitelist.</returns>
    public ItemLabels Filter();
}
