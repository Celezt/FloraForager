using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective
{
    private ObjectiveData _Data;

    private int _CurrentAmount;

    public string ItemID => _Data.ItemID;
    public int Amount => _Data.Amount;
    public int CurrentAmount
    {
        get => _CurrentAmount;
        set => _CurrentAmount = value;
    }
    public bool IsCompleted => (_CurrentAmount >= _Data.Amount);

    public Objective(ObjectiveData data)
    {
        _Data = data;
    }
}

public class FetchObjective : Objective
{
    public FetchObjective(ObjectiveData data) : base(data)
    {
        
    }

    public void UpdateItemCount()
    {
        // CurrentAmount = Inventory.Instance.GetItemCount(Type);

        ++CurrentAmount;

        CommissionLog.Instance.UpdateSelected();
        CommissionLog.Instance.CheckCompletion();
        CommissionTracker.Instance.UpdateTracker();
    }
}
