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
    public void OnBeforeSaving()
    {

    }

    public CommissionObject Object;   // associated object in the log

    public CommissionData CommissionData => _Data.Commission;

    public string Title => _Data.Commission.Title;
    public bool Repeatable => _Data.Commission.Repeatable;
    public bool HasTimeLimit => _Data.Commission.HasTimeLimit;
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

    public Commission()
    {

    }
    public Commission(CommissionData data)
    {
        _Data = new Data();

        _Data.Commission = data;

        for (int i = 0; i < _Data.Commission.Objectives.Length; ++i)
        {
            IObjective objectiveData = _Data.Commission.Objectives[i];

            _Data.Commission.Objectives[i] = (IObjective)System.Activator.CreateInstance(objectiveData.GetType()); // create new instance
            _Data.Commission.Objectives[i].Initialize(objectiveData);
        }

        _Data.DaysLeft = _Data.Commission.TimeLimit;
    }

    public void Complete()
    {
        _Data.Commission.Objectives.ForEach(o => o.Completed());

        PlayerInfo playerInfo = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>();

        Inventory inventory = playerInfo.Inventory;

        for (int i = 0; i < _Data.Commission.Rewards.Length; ++i)
        {
            ItemAsset item = _Data.Commission.Rewards[i];
            if (!inventory.Insert(item))
            {
                Vector3 dropPosition = playerInfo.transform.position;
                if (playerInfo.transform.TryGetComponent(out Collider collider))
                    dropPosition = collider.bounds.center;

                UnityEngine.Object.Instantiate(ItemTypeSettings.Instance.ItemObject, dropPosition, Quaternion.identity)
                    .Spawn(new ItemAsset { ID = item.ID, Amount = item.Amount }, playerInfo.transform.forward.xz());
            }
        }

        NPCManager.Instance.Get(Giver).Relation.AddRelation(_Data.Commission.RewardRelation);

        _Data.DaysLeft = _Data.Commission.TimeLimit;
    }

    public void DayPassed()
    {
        if (HasTimeLimit && --_Data.DaysLeft <= 0)
        {
            Penalty();
            CommissionLog.Instance.Remove(this);
        }
    }

    public void Penalty()
    {
        NPCManager.Instance.Get(Giver).Relation.AddRelation(_Data.Commission.PenaltyRelation);
        _Data.DaysLeft = _Data.Commission.TimeLimit;
    }
}
