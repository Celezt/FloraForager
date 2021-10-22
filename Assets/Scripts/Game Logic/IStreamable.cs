using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamable
{
    public void OnLoad(object state);
}

public interface IStreamable<out T> : IStreamable where T : class
{
    public T OnUpload();
}
