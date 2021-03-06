using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionExtensions
{
    public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey oldKey, TKey newKey)
    {
        TValue value;
        if (!dict.TryGetValue(oldKey, out value))
            return false;

        dict.Remove(oldKey);
        dict.Add(newKey, value);
        return true;
    }

    public static bool ChangeKey<T>(this ICollection<T> dict, T oldKey, T newKey)
    {
        if (!dict.Contains(oldKey))
            return false;

        dict.Remove(oldKey);
        dict.Add(newKey);
        return true;
    }
}
