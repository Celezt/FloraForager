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
    [System.NonSerialized]
    private Dictionary<string, NPCBehaviour> _NPCObjects = new Dictionary<string, NPCBehaviour>();

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

        SceneManager.sceneLoaded += Instance.OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<string> badKeys = _NPCObjects.Where(pair => pair.Value == null).Select(pair => pair.Key).ToList();
        foreach (string key in badKeys)
            _NPCObjects.Remove(key);
    }

    public NPC Get(string id)
    {
        if (!_NPCs.TryGetValue(id.ToLower(), out NPC npc))
            return null;

        return npc;
    }
    public NPCBehaviour GetObject(string id)
    {
        if (!_NPCObjects.TryGetValue(id.ToLower(), out NPCBehaviour npc))
            return null;

        return npc;
    }
    public void AddObject(string id, NPCBehaviour npcObject)
    {
        _NPCObjects.Add(id.ToLower(), npcObject);
    }

    public void RemoveDialogue(NPC npc, int pos)
    {
        npc.DialogueQueue.Remove(pos);
    }
    public void RemoveDialogue(NPC npc, string dialogue)
    {
        npc.DialogueQueue.Remove(dialogue);
    }
    public void RemoveDialogue(string npc, int pos)
    {
        Get(npc).DialogueQueue.Remove(pos);
    }
    public void RemoveDialogue(string npc, string dialogue)
    {
        Get(npc).DialogueQueue.Remove(dialogue);
    }

    public void EnqueueDialogue(NPC npc, string address, float priority)
    {
        npc.DialogueQueue.Enqueue(address, priority);
    }
    public void EnqueueDialogue(NPC npc, AssetReferenceText asset, float priority)
    {
        EnqueueDialogue(npc, asset.AssetGUID, priority);
    }
    public void EnqueueDialogue(string npc, string address, float priority)
    {
        Get(npc).DialogueQueue.Enqueue(address, priority);
    }
    public void EnqueueDialogue(string npc, AssetReferenceText asset, float priority)
    {
        EnqueueDialogue(npc, asset.AssetGUID, priority);
    }

    public void SetRepeatingDialogue(NPC npc, string address)
    {
        npc.SetRepeatingDialouge(address);
    }
    public void SetRepeatingDialogue(NPC npc, AssetReferenceText asset)
    {
        SetRepeatingDialogue(npc, asset.AssetGUID);
    }
    public void SetRepeatingDialogue(string npc, string address)
    {
        Get(npc).SetRepeatingDialouge(address);
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
