using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NPC : IStreamable<NPC.Data>
{
    private Data _Data;

    private Commission[] _Commissions;

    public class Data
    {
        public string Name;
        public RelationshipManager Relation;
        public CommissionData[] CommissionsData;
    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }

    public string Name => _Data.Name;
    public RelationshipManager Relation => _Data.Relation;
    public Commission[] Commissions => _Commissions;

    public NPC(NPCInfo data)
    {
        _Data = new Data();

        _Data.Name = data.Name;

        _Data.Relation = new RelationshipManager(data.RelationRange.Min, data.RelationRange.Max, data.StartRelation);

        _Commissions = new Commission[data.CommissionsData.Length];
        _Data.CommissionsData = new CommissionData[data.CommissionsData.Length];

        for (int i = 0; i < _Commissions.Length; ++i)
        {
            CommissionInfo info = data.CommissionsData[i];

            _Data.CommissionsData[i] = new CommissionData 
            {
                Title = info.Title,
                Description = info.Description,
                TimeLimit = info.TimeLimit,
                Objectives = info.Objectives,
                MinRelation = info.MinRelation,
                RewardRelation = info.RewardRelations,
                PenaltyRelation = info.PenaltyRelations,
                Rewards = info.Rewards,
                Giver = Name
            };

            _Commissions[i] = new Commission(_Data.CommissionsData[i]);
        }
    }
    public NPC(Data data) 
    {
        _Commissions = new Commission[data.CommissionsData.Length];
        for (int i = 0; i < _Commissions.Length; ++i)
        {
            _Commissions[i] = new Commission(data.CommissionsData[i]);
        }
    }
}
