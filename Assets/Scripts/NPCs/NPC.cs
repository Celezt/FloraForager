using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class NPC : IStreamable<NPC.Data>
{
    private Data _Data;

    private Commission[] _Commissions;

    private bool _HasCommissions;
    private bool _HasRelation;

    public class Data
    {
        public string Name;
        public RelationshipManager Relation;
        public CommissionData[] CommissionsData;
        public PriorityQueue<(string, string[])> DialogueQueue;
        public (string, string[]) RepeatingDialogue;
    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }
    public void OnBeforeSaving()
    {

    }

    public string Name => _Data.Name;
    public RelationshipManager Relation => _Data.Relation;
    public Commission[] Commissions => _Commissions;
    public PriorityQueue<(string, string[])> DialogueQueue => _Data.DialogueQueue;
    public (string, string[]) RepeatingDialogue => _Data.RepeatingDialogue;
    public bool HasCommissions => _HasCommissions;

    public NPC(NPCInfo data)
    {
        _HasCommissions = data.HasCommissions;
        _HasRelation = data.HasRelation;

        _Data = new Data();

        _Data.Name = data.Name;
        _Data.Relation = new RelationshipManager(data.RelationRange.Min, data.RelationRange.Max, data.StartRelation);

        // add dialogue

        _Data.DialogueQueue = new PriorityQueue<(string, string[])>(Heap.MaxHeap);
        for (int i = 0; i < data.InitialDialogue.Length; ++i)
        {
            _Data.DialogueQueue.Enqueue((
                data.InitialDialogue[i].Dialogue.AssetGUID, 
                data.InitialDialogue[i].Aliases), 
                data.InitialDialogue[i].Priority);
        }
        _Data.RepeatingDialogue = (data.RepeatingDialogue.Dialogue.AssetGUID, data.RepeatingDialogue.Aliases);

        // add commissions

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
            if (data.CommissionsData[i].Completed)
                continue;

            _Commissions[i] = new Commission(data.CommissionsData[i]);
        }
    }

    public void RemoveCommission(int pos)
    {
        _Commissions[pos] = null;
        _Data.CommissionsData[pos].Completed = true;
    }

    public void SetRepeatingDialouge(string address, params string[] aliases)
    {
        _Data.RepeatingDialogue = (address, aliases);
    }
}
