using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[System.Serializable]
public class HarvestPluck : IHarvest
{
    [OdinSerialize]
    private bool _PluckFromAll = false;
    [OdinSerialize]
    private int _PluckAmount = 1;

    public System.Action OnEmptied = delegate { };

    private int[] _Pluck;

    public IList<string> Filter(ItemLabels labels) => new List<string> { };

    public void Initialize(FloraData data)
    {
        _Pluck = System.Array.ConvertAll(data.Rewards, r => r.Amount);
    }

    public void Harvest(Flora flora, int playerIndex)
    {
        if (flora.Completed)
        {
            InventoryObject inventory = PlayerInput.GetPlayerByIndex(playerIndex).GetComponent<PlayerInfo>().Inventory;

            for (int i = 0; i < _Pluck.Length; ++i)
            {
                int amountToPluck = ((_Pluck[i] - _PluckAmount) < 0) ? _Pluck[i] : _PluckAmount;

                _Pluck[i] -= amountToPluck;

                inventory.AddItem(new ItemAsset
                {
                    ID = flora.Data.Rewards[i].ItemID,
                    Amount = amountToPluck
                });

                if (!_PluckFromAll)
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
