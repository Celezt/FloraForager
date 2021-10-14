using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Commission
{
    private readonly CommissionData _Data;    // data used to create this commission
    private readonly CommissionGiver _Giver;  // the NPC who gave out this commission
    private Objective[] _Objectives; // all of the objectives to be fulfilled to complete this commission

    private int _DaysLeft;

    public string Title => _Data.Title;
    public string Description => _Data.Description;
    public float PenaltyRelation => _Data.PenaltyRelations;
    public int DaysLeft => _DaysLeft;
    public Objective[] Objectives => _Objectives;
    public RewardPair[] Rewards => _Data.Rewards;

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

        _DaysLeft = _Data.TimeLimit;
    }

    public void Complete()
    {
        PlayerInfo playerInfo = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>();

        for (int i = 0; i < _Objectives.Length; ++i)
        {
            string itemID = _Objectives[i].ItemID;
            int amount = _Objectives[i].Amount;

            playerInfo.Inventory.Remove(itemID, amount);
        }

        for (int i = 0; i < _Data.Rewards.Length; ++i)
        {
            string itemID = _Data.Rewards[i].ItemID;
            int amount = _Data.Rewards[i].Amount;

            playerInfo.Inventory.AddItem(new ItemAsset
            {
                ID = itemID,
                Amount = amount
            });
        }

        foreach (Objective objective in Objectives)
        {
            playerInfo.Inventory.OnItemChangeCallback -= objective.UpdateAmount;
        }

        _Giver.NPC.Relations.AddRelation(_Data.RewardRelations);
    }

    public void DayPassed()
    {
        if (--_DaysLeft <= 0)
            RemoveWithPenalty();
    }

    public void RemoveWithPenalty()
    {
        _Giver.NPC.Relations.AddRelation(_Data.PenaltyRelations);
        CommissionLog.Instance.RemoveCommission(Object); // TODO: add some heads-up for the player
    }
}
