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
    [OdinSerialize]
    private System.Guid _Guid;
    [SerializeField, ListDrawerSettings(DraggableItems = false, ShowItemCount = false, Expanded = true)]
    private List<NPCInfo> _NPCInfos;

    [System.NonSerialized]
    private Dictionary<string, NPC> _NPCs = new Dictionary<string, NPC>();

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();
    }

    public void Initialize()
    {
        _NPCs = _NPCInfos.ToDictionary(n => n.Name.ToLower(), n => new NPC(n));

        foreach (KeyValuePair<string, NPC> item in _NPCs)
        {
            item.Value.Relation.UpdateRelation();
            foreach (Commission commission in item.Value.Commissions)
            {
                foreach (IObjective objective in commission.Objectives)
                {
                    objective.UpdateStatus();
                }
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void LoadInitialize()
    {
        Instance.Initialize();
        GameManager.AddStreamer(Instance);
    }

    public NPC Get(string id)
    {
        if (!_NPCs.TryGetValue(id.ToLower(), out NPC npc))
            return null;

        return npc;
    }

    public NPC Add(string id, NPC npc)
    {
        return _NPCs[id.ToLower()] = npc;
    }

    public void RemoveDialogue(NPC npc, int pos)
    {
        npc.DialogueQueue.Remove(pos);
    }
    public void RemoveDialogue(NPC npc, (string, string[]) dialogue)
    {
        npc.DialogueQueue.Remove(dialogue);
    }
    public void RemoveDialogue(string npc, int pos)
    {
        Get(npc).DialogueQueue.Remove(pos);
    }
    public void RemoveDialogue(string npc, (string, string[]) dialogue)
    {
        Get(npc).DialogueQueue.Remove(dialogue);
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
        Get(npc).DialogueQueue.Enqueue((address, aliases), priority);
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
        Get(npc).SetRepeatingDialouge(address, aliases);
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
        Initialize();

        if (!GameManager.Stream.TryGet(_Guid, out Dictionary<string, object> streamables))
            return;

        foreach (KeyValuePair<string, object> item in streamables)
        {
            if (!streamables.TryGetValue(item.Key, out object value))
                continue;

            NPC.Data data = value as NPC.Data;

            Get(data.Name).OnLoad(data);
        }
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();

        foreach (KeyValuePair<string, NPC> item in _NPCs)
        {
            item.Value.OnBeforeSaving();
        }
    }
}
