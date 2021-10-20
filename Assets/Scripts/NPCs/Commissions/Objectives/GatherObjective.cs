using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;

[System.Serializable]
public class GatherObjective : IObjective
{
    public string ItemID;
    public int Amount;

    private InventoryObject _Inventory;
    private int _CurrentAmount;

    public bool IsCompleted => (_CurrentAmount >= Amount);
    public string Status => ItemID + ": " + _CurrentAmount + "/" + Amount;

    public void Initialize(IObjective objectiveData)
    {
        _Inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;
        _Inventory.OnItemChangeCallback += UpdateStatus;

        GatherObjective gatheringObjective = objectiveData as GatherObjective;

        ItemID = gatheringObjective.ItemID;
        Amount = gatheringObjective.Amount;

        _CurrentAmount = _Inventory.FindAmount(ItemID);
    }

    public void UpdateStatus() { UpdateStatus(0); }

    public void UpdateStatus(int pos)
    {
        _CurrentAmount = _Inventory.FindAmount(ItemID);

        CommissionLog.Instance.UpdateSelected();
        CommissionLog.Instance.CheckCompletion();
        CommissionTracker.Instance.UpdateTracker();
    }

    public void Complete()
    {
        _Inventory.Remove(ItemID, Amount);
        _Inventory.OnItemChangeCallback -= UpdateStatus;
    }
}
