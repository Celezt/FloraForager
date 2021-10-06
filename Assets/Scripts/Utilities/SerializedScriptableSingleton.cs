using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;

public class SerializedScriptableSingleton<T> : SerializedScriptableObject where T : SerializedScriptableSingleton<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] assets = Resources.LoadAll<T>("");
                if (assets == null | (assets != null && assets.Length < 1))
                    throw new Exception($"Could not find any singleton serialized scriptable object instances of type {typeof(T)}.");
                else if (assets.Length > 1)
                    Debug.LogWarning($"Multiple instances of the singleton serialized scriptable object of type {typeof(T)} found in the project.");

                _instance = assets[0];
            }

            return _instance;
        }
    }
}
