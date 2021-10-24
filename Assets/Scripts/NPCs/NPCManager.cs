using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "FloraMaster", menuName = "Scriptable Objects/NPC Manager")]
[System.Serializable]
public class NPCManager : SerializedScriptableSingleton<NPCManager>
{
    [SerializeField]
    private System.Guid _Guid;

    private Dictionary<string, NPC> _NPCs = new Dictionary<string, NPC>();

    public bool Exists(string npc)
    {
        return _NPCs.ContainsKey(npc);
    }

    public NPC Get(string npc)
    {
        return _NPCs[npc];
    }

    public NPC Add(string id, NPC npc)
    {
        return _NPCs[id] = npc;
    }

    public object Upload()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        _NPCs.ForEach(x => streamables.Add(x.Key, ((IStreamable<object>)x.Value).OnUpload()));

        GameManager.Stream.Load(_Guid, streamables);

        return null;
    }
    public void Load()
    {
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
}
