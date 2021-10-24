using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MyBox;

public class Commission : IStreamable<Commission.Data>
{
    private Data _Data;

    public class Data
    {
        public CommissionData Commission;
        public int DaysLeft;
    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }

    public CommissionObject Object;   // associated object in the log

    public CommissionData CommissionData => _Data.Commission;

    public string Title => _Data.Commission.Title;
    public IObjective[] Objectives => _Data.Commission.Objectives;
    public string Giver => _Data.Commission.Giver;
    public int DaysLeft => _Data.DaysLeft;

    public bool IsCompleted
    {
        get
        {
            foreach (IObjective objective in Objectives)
            {
                if (!objective.IsCompleted)
                    return false;
            }
            return true;
        }
    }

    public Commission(CommissionData data)
    {
        _Data = new Data();

        _Data.Commission = data;

        for (int i = 0; i < _Data.Commission.Objectives.Length; ++i)
        {
            IObjective objectiveData = _Data.Commission.Objectives[i];

            _Data.Commission.Objectives[i] = (IObjective)System.Activator.CreateInstance(objectiveData.GetType()); // create new instance
            _Data.Commission.Objectives[i].Initialize(objectiveData); // fill it with data
        }

        _Data.DaysLeft = _Data.Commission.TimeLimit;
    }

    public void Complete()
    {
        _Data.Commission.Objectives.ForEach(o => o.Completed());

        Inventory inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;

        for (int i = 0; i < _Data.Commission.Rewards.Length; ++i)
            inventory.Insert(_Data.Commission.Rewards[i].ItemID, _Data.Commission.Rewards[i].Amount);

        NPCManager.Instance.Get(Giver).Relation.AddRelation(_Data.Commission.RewardRelation);
    }

    public void DayPassed()
    {
        if (--_Data.DaysLeft <= 0)
        {
            Penalty();
            CommissionLog.Instance.Remove(this);
        }
    }

    public void Penalty()
    {
        NPCManager.Instance.Get(Giver).Relation.AddRelation(_Data.Commission.PenaltyRelation);
    }
}
