using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtility
{
    public static IEnumerable<Type> GetTypesWithAttribute<T>(Assembly assembly) where T : Attribute
    {
        return assembly.GetTypes().Where(t => Attribute.IsDefined(t, typeof(T)));
    }

    private static IEnumerable<Type> GetDerivedTypes<T>(Assembly assembly)
    {
        return assembly.GetTypes().Where(t => t != typeof(T) && typeof(T).IsAssignableFrom(t));
    }
}
