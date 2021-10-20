using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MyBox;

public class Commission
{
    private CommissionData _Data;     // data used to create this commission

    private IObjective[] _Objectives; // all of the objectives to be fulfilled to complete this commission
    private NPC _Giver;               // the NPC who gave out this commission
    private int _DaysLeft;

    public CommissionObject Object;   // associated object in the log

    public CommissionData Data => _Data;

    public IObjective[] Objectives => _Objectives;
    public NPC Giver => _Giver;
    public int DaysLeft => _DaysLeft;

    public bool IsCompleted
    {
        get
        {
            foreach (IObjective objective in _Objectives)
            {
                if (!objective.IsCompleted)
                    return false;
            }
            return true;
        }
    }

    public Commission(CommissionData data, NPC giver)
    {
        _Data = data;
        _Giver = giver;

        _Objectives = new IObjective[_Data.Objectives.Length]; // create objectives based on assigned objectives data
        for (int i = 0; i < _Objectives.Length; ++i)
        {
            IObjective objectiveData = _Data.Objectives[i];

            _Objectives[i] = (IObjective)System.Activator.CreateInstance(objectiveData.GetType()); // create new instance
            _Objectives[i].Initialize(objectiveData); // fill it with data
        }

        _DaysLeft = _Data.TimeLimit;
    }

    public void Complete()
    {
        _Objectives.ForEach(o => o.Complete());

        Inventory inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;
        for (int i = 0; i < _Data.Rewards.Length; ++i)
        {
            string itemID = _Data.Rewards[i].ItemID;
            int amount = _Data.Rewards[i].Amount;

            inventory.Insert(new ItemAsset
            {
                ID = itemID,
                Amount = amount
            });
        }

        _Giver.Relation.AddRelation(_Data.RewardRelations);
    }

    public void DayPassed()
    {
        if (--_DaysLeft <= 0)
        {
            Penalty();
            CommissionLog.Instance.Remove(this);
        }
    }

    public void Penalty()
    {
        _Giver.Relation.AddRelation(_Data.PenaltyRelations);
    }
}
