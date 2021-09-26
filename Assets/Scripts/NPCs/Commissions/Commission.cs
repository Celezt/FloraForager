using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Commission
{
    private CommissionData _Data;
    private Objective[] _Objectives;

    public string Title => _Data.Title;
    public string Description => _Data.Description;
    public Objective[] Objectives => _Objectives;

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

    public Commission(CommissionData data)
    {
        _Data = data;

        _Objectives = new Objective[_Data.ObjectivesData.Length];
        for (int i = 0; i < _Objectives.Length; ++i)
        {
            _Objectives[i] = new FetchObjective(_Data.ObjectivesData[i]);
        }
    }

    public void Complete()
    {
        for (int i = 0; i < _Objectives.Length; ++i)
        {
            string itemID = _Objectives[i].Type;
            int amount = _Objectives[i].Amount;

            // get items and remove
        }

        for (int i = 0; i < _Data.Rewards.Length; ++i)
        {
            string itemID = _Data.Rewards[i].ID;
            int amount = _Data.Rewards[i].Amount;

            // add items to inventory
        }
    }
}
