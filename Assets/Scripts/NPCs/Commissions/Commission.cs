using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Commission
{
    private CommissionData _Data;    // data used to create this commission
    private CommissionGiver _Giver;  // the NPC who gave out this commission
    private Objective[] _Objectives; // all of the objectives to be fulfilled to complete this commission

    public string Title => _Data.Title;
    public string Description => _Data.Description;
    public Objective[] Objectives => _Objectives;
    public RewardPair<string, int>[] Rewards => _Data.Rewards;

    public CommissionGiver Giver => _Giver;

    public CommissionObject Object { get; set; }

    public bool IsCompleted
    {
        get
        {
            foreach (Objective objective in _Objectives)
            {
                if (!objective.IsCompleted)
                    return false;
            }
            return true;
        }
    }

    public Commission(CommissionData data, CommissionGiver giver)
    {
        _Data = data;
        _Giver = giver;

        _Objectives = new Objective[_Data.ObjectivesData.Length]; // create objectives based on assigned objectives data
        for (int i = 0; i < _Objectives.Length; ++i)
        {
            _Objectives[i] = new FetchObjective(_Data.ObjectivesData[i]);
        }
    }

    public void Complete()
    {
        for (int i = 0; i < _Objectives.Length; ++i)
        {
            string itemID = _Objectives[i].ItemID;
            int amount = _Objectives[i].Amount;

            // get items and remove
        }

        for (int i = 0; i < _Data.Rewards.Length; ++i)
        {
            string itemID = _Data.Rewards[i].ItemID;
            int amount = _Data.Rewards[i].Amount;

            // add items to inventory
        }
    }
}
