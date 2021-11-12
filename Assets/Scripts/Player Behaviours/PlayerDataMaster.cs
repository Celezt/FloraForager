using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlayerDataMaster", menuName = "Game Logic/Player Data Master")]
[System.Serializable]
public class PlayerDataMaster : SerializedScriptableSingleton<PlayerDataMaster>, IStreamer
{
    [SerializeField]
    private System.Guid _Guid;

    [System.NonSerialized]
    private Dictionary<int, PlayerData> _Players = new Dictionary<int, PlayerData>();

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();

        GameManager.AddStreamer(this);
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void Initialize()
    {
        GameManager.AddStreamer(Instance);
    }
#endif

    public PlayerData Get(int playerIndex)
    {
        if (_Players.ContainsKey(playerIndex))
            return _Players[playerIndex];

        PlayerData data = new PlayerData(playerIndex);

        return _Players[playerIndex] = data;
    }

    public bool Exists(int playerIndex)
    {
        return _Players.ContainsKey(playerIndex);
    }

    public void UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        foreach (KeyValuePair<int, PlayerData> item in _Players)
        {
            streamables.Add(item.Key.ToString(), item.Value.OnUpload());
        }

        GameManager.Stream.Load(_Guid, streamables);
    }
    public void Load()
    {
        _Players.Clear();

        if (!GameManager.Stream.TryGet(_Guid, out Dictionary<string, object> streamables))
            return;

        foreach (KeyValuePair<string, object> item in streamables)
        {
            if (!streamables.TryGetValue(item.Key, out object value))
                continue;

            if (!int.TryParse(item.Key, out int playerIndex))
                continue;

            PlayerData data = new PlayerData(playerIndex);
            data.OnLoad(value);

            _Players[playerIndex] = data;
        }
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();

        foreach (KeyValuePair<int, PlayerData> item in _Players)
        {
            item.Value.OnBeforeSaving();
        }
    }
}
