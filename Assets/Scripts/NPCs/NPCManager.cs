using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MyBox;

public class NPCManager : Singleton<NPCManager>
{
    private static Dictionary<string, NPC> _NPCs = new Dictionary<string, NPC>();

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
}
