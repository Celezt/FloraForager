using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamable
{
    /// <summary>
    /// Stream new data to data manager.
    /// </summary>
    /// <returns></returns>
    public object Stream();
}
