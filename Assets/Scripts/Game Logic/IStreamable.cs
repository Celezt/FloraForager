using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamable
{
    /// <summary>
    /// Load existing reference from streamables. Does not get called if no reference exist.
    /// </summary>
    /// <param name="state"></param>
    public void OnLoad(object state);
    /// <summary>
    /// Called before saving for last changes.
    /// </summary>
    public void OnBeforeSaving();
}

public interface IStreamable<out T> : IStreamable where T : class
{
    /// <summary>
    /// Upload reference to streamables.
    /// </summary>
    /// <returns></returns>
    public T OnUpload();
}
