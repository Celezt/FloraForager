using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Taggable : IPackage
{
    private object _wrapped;

    public void Wrap<T>(T wrapped) => _wrapped = wrapped;
    public T Unwrap<T>() => (T)_wrapped;

    public static Taggable CreatePackage<T>(T wrapped)
    {
        Taggable taggable = new Taggable();
        taggable.Wrap(wrapped);
        return taggable;
    }
}
