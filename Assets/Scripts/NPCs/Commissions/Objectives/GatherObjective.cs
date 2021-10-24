using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;

[System.Serializable]
public class GatherObjective : IObjective
{
    public string ItemID;
    public int Amount;

    private string _ItemName;

    private Inventory _Inventory;
    private int _CurrentAmount;

    public bool IsCompleted => (_CurrentAmount >= Amount);
    public string Status => $"{_ItemName}: {_CurrentAmount}/{Amount}";

    public void Initialize(IObjective objectiveData)
    {
        GatherObjective gatheringObjective = objectiveData as GatherObjective;

        ItemID = gatheringObjective.ItemID;
        Amount = gatheringObjective.Amount;

        _ItemName = ItemTypeSettings.Instance.ItemNameChunk[ItemID];
    }

    public void Accepted()
    {
        _Inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;
        _Inventory.OnItemChangeCallback += UpdateStatus;

        _CurrentAmount = _Inventory.FindAmount(ItemID);
    }
    public void Removed()
    {
        _Inventory.OnItemChangeCallback -= UpdateStatus;
    }
    public void Completed()
    {
        _Inventory.Remove(ItemID, Amount);
    }

    public void UpdateStatus() { UpdateStatus(0, new ItemAsset { }); }
    public void UpdateStatus(int pos, ItemAsset item)
    {
        _CurrentAmount = _Inventory.FindAmount(ItemID);

        CommissionLog.Instance.UpdateSelected();
        CommissionLog.Instance.CheckCompletion();
        CommissionTracker.Instance.UpdateTracker();
    }
}
