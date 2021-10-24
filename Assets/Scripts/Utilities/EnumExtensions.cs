using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnumExtensions
{
    /// <summary>
    /// Extract all flags and return singles.
    /// </summary>
    public static T[] Extract<T>(this T e) where T : Enum => 
        Enum.GetValues(typeof(T))
               .Cast<T>()
               .Where(c => e.HasFlag(c))
               .ToArray();
}
