using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Taggable : IPackage
{
    public int Layer => _layer;
    public bool IsCancelled => _isCancelled;

    private bool _isCancelled;
    private int _layer;
    private object _wrapped;

    public void Wrap<T>(T wrapped, int layer)
    {
        _wrapped = wrapped;
        _layer = layer;
        _isCancelled = false;
    }

    public T Unwrap<T>() => (T)_wrapped;

    public static Taggable CreatePackage<T>(T wrapped, int layer, bool isCancelled = false)
    {
        Taggable taggable = new Taggable();
        taggable.Wrap(wrapped, layer);
        taggable._isCancelled = isCancelled;
        return taggable;
    }
}
