using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using MyBox;

[System.Serializable]
public class HarvestPluck : IHarvest
{
    public bool PluckFromAll = false;
    public int PluckAmount = 1;

    [ListDrawerSettings(ShowItemCount = false, DraggableItems = false, Expanded = true)]
    public ItemAsset[] Rewards;

    private int[] _Pluck;

    [HideInInspector]
    public event System.Action OnEmptied = delegate { };

    public void Initialize(FloraInfo data, IHarvest harvestData)
    {
        HarvestPluck harvestPluck = harvestData as HarvestPluck;

        PluckFromAll = harvestPluck.PluckFromAll;
        PluckAmount = harvestPluck.PluckAmount;
        Rewards = harvestPluck.Rewards;

        _Pluck = System.Array.ConvertAll(Rewards, r => r.Amount);
    }

    public bool Harvest(UsedContext context, Flora flora)
    {
        if (!flora.Completed)
            return false;

        Inventory inventory = PlayerInput.GetPlayerByIndex(context.playerIndex).GetComponent<PlayerInfo>().Inventory;

        for (int i = 0; i < _Pluck.Length; ++i)
        {
            if (_Pluck[i] <= 0)
                continue;

            int emptySpace = inventory.FindEmptySpace(Rewards[i].ID);
            int amountToPluck = ((_Pluck[i] - PluckAmount) < 0) ? _Pluck[i] : PluckAmount;

            if (emptySpace >= amountToPluck)
            {
                _Pluck[i] -= amountToPluck;

                inventory.Insert(Rewards[i].ID, amountToPluck);

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
