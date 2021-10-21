using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamableObject
{

}

public interface IStreamableObject<out T> : IStreamableObject where T : class, IStreamable
{
    /// <summary>
    /// Streamable data.
    /// </summary>
    /// <returns></returns>
    public T Data();
}
