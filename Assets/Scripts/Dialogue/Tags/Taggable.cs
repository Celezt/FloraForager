using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Taggable : IPackage
{
    public int Layer => _layer;

    private int _layer;
    private object _wrapped;

    public void Wrap<T>(T wrapped, int layer)
    {
        _wrapped = wrapped;
        _layer = layer;
    }

    public T Unwrap<T>() => (T)_wrapped;

    public static Taggable CreatePackage<T>(T wrapped, int layer)
    {
        Taggable taggable = new Taggable();
        taggable.Wrap(wrapped, layer);
        return taggable;
    }
}
