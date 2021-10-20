using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NPC
{
    private NPCData _NPCData;

    public string Name => _NPCData.Name;

    public RelationshipManager Relation;
    public Commission[] Commissions;

    public NPC(NPCData data)
    {
        _NPCData = data;

        Relation = new RelationshipManager(data.RelationRange.Min, data.RelationRange.Max, data.StartRelation);

        Commissions = new Commission[data.CommissionsData.Length];
        for (int i = 0; i < Commissions.Length; ++i)
        {
            Commissions[i] = new Commission(data.CommissionsData[i], this);
        }
    }
}
