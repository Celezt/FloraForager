using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaceable
{
    /// <summary>
    /// If placed.
    /// </summary>
    /// <param name="context"></param>
    public void OnPlace(PlacedContext context);
}
