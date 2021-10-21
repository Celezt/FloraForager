using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamableObject
{
    public void OnLoad(object state);
}

public interface IStreamableObject<out T> : IStreamableObject where T : class
{
    public T OnUnload();
}
