using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[System.Serializable]
public class HarvestPluck : IHarvest
{
    public bool PluckFromAll = false;
    public int PluckAmount = 1;

    private int[] _Pluck;

    [HideInInspector]
    public System.Action OnEmptied = delegate { };

    public void Initialize(FloraInfo data, IHarvest harvestData)
    {
        _Pluck = System.Array.ConvertAll(data.Rewards, r => r.Amount);

        HarvestPluck harvestPluck = harvestData as HarvestPluck;

        PluckFromAll = harvestPluck.PluckFromAll;
        PluckAmount = harvestPluck.PluckAmount;
    }

    public bool Harvest(Flora flora, int playerIndex)
    {
        if (!flora.Completed)
            return false;

        Inventory inventory = PlayerInput.GetPlayerByIndex(playerIndex).GetComponent<PlayerInfo>().Inventory;

        for (int i = 0; i < _Pluck.Length; ++i)
        {
            if (_Pluck[i] <= 0)
                continue;

            int emptySpace = inventory.FindEmptySpace(flora.FloraInfo.Rewards[i].ItemID);
            int amountToPluck = ((_Pluck[i] - PluckAmount) < 0) ? _Pluck[i] : PluckAmount;

            if (emptySpace >= amountToPluck)
            {
                _Pluck[i] -= amountToPluck;

                inventory.Insert(new ItemAsset
                {
                    ID = flora.FloraInfo.Rewards[i].ItemID,
                    Amount = amountToPluck
                });

                if (!PluckFromAll)
                    break;
            }
        }

        if (_Pluck.All(p => p <= 0)) // if nothing more to pluck
        {
            OnEmptied.Invoke();
            return true;
        }

        return false;
    }
}
