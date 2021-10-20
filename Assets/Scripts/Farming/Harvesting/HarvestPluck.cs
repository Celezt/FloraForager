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

    [HideInInspector]
    public System.Action OnEmptied = delegate { };

    private int[] _Pluck;

    public IList<string> Filter(ItemLabels labels) => new List<string> { };

    public void Initialize(FloraData data, IHarvest harvestData)
    {
        _Pluck = System.Array.ConvertAll(data.Rewards, r => r.Amount);

        HarvestPluck harvestPluck = harvestData as HarvestPluck;

        PluckFromAll = harvestPluck.PluckFromAll;
        PluckAmount = harvestPluck.PluckAmount;
    }

    public void Harvest(Flora flora, int playerIndex)
    {
        if (flora.Completed)
        {
            InventoryObject inventory = PlayerInput.GetPlayerByIndex(playerIndex).GetComponent<PlayerInfo>().Inventory;

            for (int i = 0; i < _Pluck.Length; ++i)
            {
                int amountToPluck = ((_Pluck[i] - PluckAmount) < 0) ? _Pluck[i] : PluckAmount;

                _Pluck[i] -= amountToPluck;

                inventory.AddItem(new ItemAsset
                {
                    ID = flora.Data.Rewards[i].ItemID,
                    Amount = amountToPluck
                });

                if (!PluckFromAll)
                    break;
            }

            if (_Pluck.All(p => p <= 0)) // if nothing more to pluck
            {
                OnEmptied.Invoke();

                UnityEngine.Object.Destroy(Grid.Instance.FreeCell(flora.Cell));
                FloraMaster.Instance.Remove(flora);
            }
        }
    }

    public void OnUse(UsedContext context) { }
}
