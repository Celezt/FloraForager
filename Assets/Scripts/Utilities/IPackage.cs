using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPackage
{
    public void Wrap<T>(T wrapped);
    public T Unwrap<T>();
}
