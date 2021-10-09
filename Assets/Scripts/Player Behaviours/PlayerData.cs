using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/Player Data")]
[System.Serializable]
public class PlayerData : ScriptableObject
{
    public PlayerAsset Data;

    public void Load()
    {
        Addressables.LoadAssetAsync<TextAsset>("player").Completed += (handle) =>
        {
            Debug.Log(handle.Result.text);
            Data = JsonConvert.DeserializeObject<PlayerAsset>(handle.Result.text);
        };
    }

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        PlayerData[] data = GameObject.FindObjectsOfType<PlayerData>();
        for (int i = 0; i < data.Length; i++)
            data[i].Load();
    }
}
