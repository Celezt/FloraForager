using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HarvestScythe : IHarvest
{
    public IList<string> Filter(ItemLabels labels) => new List<string> { labels.SCYTHE };

    public void Initialize(FloraData data, IHarvest harvestData)
    {
        
    }

    public void Harvest(Flora flora, int playerIndex)
    {
        if (flora.Completed)
        {
            Inventory inventory = PlayerInput.GetPlayerByIndex(playerIndex).GetComponent<PlayerInfo>().Inventory;

            foreach (RewardPair reward in flora.Data.Rewards)
            {
                inventory.Insert(new ItemAsset
                {
                    ID = reward.ItemID,
                    Amount = reward.Amount
                });
            }
        }

        UnityEngine.Object.Destroy(Grid.Instance.FreeCell(flora.Cell));
        FloraMaster.Instance.Remove(flora);
    }

    public void OnUse(UsedContext context) { }
}
