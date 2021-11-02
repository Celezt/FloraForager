using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[System.Serializable]
public class GatherObjective : IObjective
{
    public ItemAsset ItemToGather;
    [HideInInspector]
    public int CurrentAmount;

    private Inventory _Inventory;
    private string _ItemName;

    public bool IsCompleted => (CurrentAmount >= ItemToGather.Amount);
    public string Status => $"{_ItemName}: {CurrentAmount}/{ItemToGather.Amount}";

    public void Initialize(IObjective objectiveData)
    {
        GatherObjective gatheringObjective = objectiveData as GatherObjective;

        ItemToGather = gatheringObjective.ItemToGather;
        CurrentAmount = gatheringObjective.CurrentAmount;

        _ItemName = ItemTypeSettings.Instance.ItemNameChunk[ItemToGather.ID];
    }

    public void Accepted()
    {
        _Inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;

        _Inventory.OnAddItemCallback += UpdateStatus;
        _Inventory.OnRemoveItemCallback += UpdateStatus;

        UpdateStatus();
    }
    public void Removed()
    {
        _Inventory.OnAddItemCallback -= UpdateStatus;
        _Inventory.OnRemoveItemCallback -= UpdateStatus;
    }
    public void Completed()
    {
        _Inventory.Remove(ItemToGather.ID, ItemToGather.Amount);
    }

    public void UpdateStatus() { UpdateStatus(0, new ItemAsset { }); }
    public void UpdateStatus(int pos, ItemAsset item)
    {
        CurrentAmount = _Inventory.FindAmount(ItemToGather.ID);

        CommissionLog.Instance.UpdateSelected();
        CommissionLog.Instance.CheckCompletion();
        CommissionTracker.Instance.UpdateTracker();
    }
}
