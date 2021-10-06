using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public static class ObjectExtensions
{

    /// <summary>
    /// Adds newly (if not already in the list) found assets.
    /// Returns how many found (not how many added)
    /// </summary>
    /// <see cref="https://forum.unity.com/threads/loadallassetsatpath-not-working-or-im-using-it-wrong.110326/"/>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="assetsFound">Adds to this list if it is not already there</param>
    /// <returns></returns>
    public static int TryGetObjectsOfTypeFromPath<T>(string path, List<T> assetsFound) where T : Object
    {
        string[] filePaths = System.IO.Directory.GetFiles(path);

        int countFound = 0;

        if (filePaths != null && filePaths.Length > 0)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(T));
                if (obj is T asset)
                {
                    countFound++;
                    if (!assetsFound.Contains(asset))
                    {
                        assetsFound.Add(asset);
                    }
                }
            }
        }

        return countFound;
    }

}
#endif
