using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NPC Manager", menuName = "Game Logic/NPC Manager")]
[System.Serializable]
public class NPCManager : SerializedScriptableSingleton<NPCManager>, IStreamer
{
    [SerializeField]
    private System.Guid _Guid;
    [SerializeField]
    private List<NPCInfo> _NPCInfos;

    [System.NonSerialized]
    private Dictionary<string, NPC> _NPCs = new Dictionary<string, NPC>();

    public void Initialize()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();

        _NPCs = _NPCInfos.ToDictionary(n => n.Name.ToLower(), n => new NPC(n));

        GameManager.AddStreamer(this);
    }

    private void Awake()
    {
        Initialize();
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void PlayModeInitialize()
    {
        Instance.Initialize();
    }
#endif

    public NPC Get(string npc)
    {
        return _NPCs[npc.ToLower()];
    }

    public NPC Add(string id, NPC npc)
    {
        return _NPCs[id.ToLower()] = npc;
    }

    public void EnqueueDialogue(string npc, string address, float priority)
    {
        _NPCs[npc].DialogueQueue.Enqueue(address, priority);
    }
    public void EnqueueDialogue(string npc, AssetReferenceText asset, float priority)
    {
        EnqueueDialogue(npc, asset.AssetGUID, priority);
    }

    public void SetRepeatingDialogue(string npc, string address)
    {
        _NPCs[npc].SetRepeatingDialouge(address);
    }
    public void SetRepeatingDialogue(string npc, AssetReferenceText asset)
    {
        SetRepeatingDialogue(npc, asset.AssetGUID);
    }

    public void UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        _NPCs.ForEach(x => streamables.Add(x.Key, ((IStreamable<object>)x.Value).OnUpload()));

        GameManager.Stream.Load(_Guid, streamables);
    }
    public void Load()
    {
        _NPCs = new Dictionary<string, NPC>();

        Dictionary<string, object> streamables = (Dictionary<string, object>)GameManager.Stream.Get(_Guid);

        foreach (var item in streamables)
        {
            if (!streamables.TryGetValue(item.Key, out object value))
                continue;

            NPC.Data data = value as NPC.Data;

            NPC npc = new NPC(data);
            npc.OnLoad(data);

            Add(npc.Name, npc);
        }
    }
    public void BeforeSaving()
    {

    }
}
