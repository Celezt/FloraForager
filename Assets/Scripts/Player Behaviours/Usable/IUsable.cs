using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable : IPlayerInteractable
{
    public void OnUse(UsedContext context);

    /// <summary>
    /// Whitelist users with label.
    /// </summary>
    /// <param name="labels">Auto generated labels</param>
    /// <returns>To whitelist.</returns>
    public IList<string> Filter(ItemLabels labels);
}
