using IngameDebugConsole;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class FileConsoleUtility
{
    [ConsoleMethod("file.read_json", "Read JSON-file")]
    public static void ReadJSON(string address)
    {
        Addressables.LoadAssetAsync<TextAsset>(address).Completed += (handle) =>
        {
            Debug.Log(handle.Result);

            Addressables.Release(handle);
        };
    }
}
