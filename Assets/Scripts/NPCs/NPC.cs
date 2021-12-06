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
        public string RepeatingDialogue;
        public PriorityQueue<string> DialogueQueue;
        public DialogueRelationSave[] DialogueRelations;
    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;

        _Commissions = new Commission[_Data.CommissionsData.Length];
        for (int i = 0; i < _Commissions.Length; ++i)
        {
            if (_Data.CommissionsData[i].Completed)
                continue;

            _Commissions[i] = new Commission(_Data.CommissionsData[i]);
        }
    }
    public void OnBeforeSaving()
    {

    }

    public string Name => _Data.Name;
    public RelationshipManager Relation => _Data.Relation;
    public Commission[] Commissions => _Commissions;
    public string RepeatingDialogue => _Data.RepeatingDialogue;
    public PriorityQueue<string> DialogueQueue => _Data.DialogueQueue;
    public DialogueRelationSave[] DialogueRelations => _Data.DialogueRelations;
    public bool HasCommissions => _HasCommissions;

    public NPC(NPCInfo data)
    {
        _HasCommissions = data.HasCommissions;
        _HasRelation = data.HasRelation;

        _Data = new Data();

        _Data.Name = data.Name;

        // add dialogue

        _Data.RepeatingDialogue = data.RepeatingDialogue?.AssetGUID;

        _Data.DialogueQueue = new PriorityQueue<string>(Heap.MaxHeap);
        for (int i = 0; i < data.InitialDialogue.Length; ++i)
        {
            _Data.DialogueQueue.Enqueue(
                data.InitialDialogue[i].Dialogue?.AssetGUID, 
                data.InitialDialogue[i].Priority);
        }

        _Data.DialogueRelations = Array.ConvertAll(data.RelationDialogue, dialogue => new DialogueRelationSave 
        {  
            AtRelation = dialogue.AtRelation,
            RepeatingDialogue = dialogue.RepeatingDialogue.AssetGUID,
            AddedDialogue = Array.ConvertAll(dialogue.AddedDialogue, addedDialogue => (
                addedDialogue.Priority, 
                addedDialogue.Dialogue?.AssetGUID))
        });

        _Data.Relation = new RelationshipManager(Name, data.RelationRange.Min, data.RelationRange.Max, data.StartRelation);

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
                Repeatable = info.Repeatable,
                TimeLimit = info.TimeLimit,
                Objectives = info.Objectives,
                MinRelation = info.MinRelation,
                RewardRelation = info.RewardRelations,
                PenaltyRelation = info.PenaltyRelations,
                Rewards = info.Rewards,
                AcceptDialogue = info.AcceptDialogue.ToDictionary(
                    d => d.NPC.ToLower(), 
                    d => d.Dialogue.AssetGUID),
                CompleteDialogue = info.CompleteDialogue.ToDictionary(
                    d => d.NPC.ToLower(), 
                    d => d.Dialogue.AssetGUID),
                Giver = Name,
                Completed = false
            };

            _Commissions[i] = new Commission(_Data.CommissionsData[i]);
        }
    }

    public void SetCommission(Commission commission)
    {
        for (int i = _Commissions.Length - 1; i >= 0; --i)
        {
            Commission npcCommission = _Commissions[i];

            if (npcCommission == null)
                continue;

            if (npcCommission.Title == commission.Title && npcCommission.Giver == commission.Giver)
            {
                _Commissions[i] = commission;
            }
        }
    }

    public void OpenCommissions()
    {
        if (HasCommissions)
        {
            CommissionGiverWindow.Instance.ShowCommissions(this);
            CommissionGiverWindow.Instance.Open();
        }
    }
    public void CloseCommissions()
    {
        if (HasCommissions)
        {
            CommissionGiverWindow.Instance.Exit();
        }
    }

    public void RemoveCommission(int pos)
    {
        _Commissions[pos] = null;
        _Data.CommissionsData[pos].Completed = true;
    }

    public void SetRepeatingDialouge(string address)
    {
        _Data.RepeatingDialogue = address;
    }
}
