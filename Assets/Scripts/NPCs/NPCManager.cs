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

    public void EnqueueDialogue(NPC npc, string address, float priority, params string[] aliases)
    {
        npc.DialogueQueue.Enqueue((address, aliases), priority);
    }
    public void EnqueueDialogue(NPC npc, AssetReferenceText asset, float priority, params string[] aliases)
    {
        EnqueueDialogue(npc, asset.AssetGUID, priority, aliases);
    }
    public void EnqueueDialogue(string npc, string address, float priority, params string[] aliases)
    {
        _NPCs[npc].DialogueQueue.Enqueue((address, aliases), priority);
    }
    public void EnqueueDialogue(string npc, AssetReferenceText asset, float priority, params string[] aliases)
    {
        EnqueueDialogue(npc, asset.AssetGUID, priority, aliases);
    }

    public void SetRepeatingDialogue(NPC npc, string address, params string[] aliases)
    {
        npc.SetRepeatingDialouge(address, aliases);
    }
    public void SetRepeatingDialogue(NPC npc, AssetReferenceText asset, params string[] aliases)
    {
        SetRepeatingDialogue(npc, asset.AssetGUID, aliases);
    }
    public void SetRepeatingDialogue(string npc, string address, params string[] aliases)
    {
        _NPCs[npc].SetRepeatingDialouge(address, aliases);
    }
    public void SetRepeatingDialogue(string npc, AssetReferenceText asset, params string[] aliases)
    {
        SetRepeatingDialogue(npc, asset.AssetGUID, aliases);
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
