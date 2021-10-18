using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Objective
{
    private ObjectiveData _Data;

    public int CurrentAmount { get; set; }

    public string ItemID => _Data.ItemID;
    public int Amount => _Data.Amount;

    public bool IsCompleted => (CurrentAmount >= _Data.Amount);

    public Objective(ObjectiveData data)
    {
        _Data = data;
    }

    public abstract void UpdateAmount(int pos, ItemAsset item);
}

public class FetchObjective : Objective
{
    private Inventory _Inventory;

    public FetchObjective(ObjectiveData data) : base(data)
    {
        _Inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;
    }

    public override void UpdateAmount(int pos, ItemAsset item)
    {
        CurrentAmount = _Inventory.FindAmount(ItemID);

        CommissionLog.Instance.UpdateSelected();
        CommissionLog.Instance.CheckCompletion();
        CommissionTracker.Instance.UpdateTracker();
    }
}
