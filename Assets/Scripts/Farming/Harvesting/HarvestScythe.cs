using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HarvestScythe : IHarvest
{
    public IList<string> Filter(ItemLabels labels) => new List<string> { labels.SCYTHE };

    public void Initialize(FloraData data)
    {
        
    }

    public void Harvest(Flora flora, int playerIndex)
    {
        if (flora.Completed)
        {
            InventoryObject inventory = PlayerInput.GetPlayerByIndex(playerIndex).GetComponent<PlayerInfo>().Inventory;

            foreach (RewardPair reward in flora.Data.Rewards)
            {
                inventory.AddItem(new ItemAsset
                {
                    ID = reward.ItemID,
                    Amount = reward.Amount
                });
            }
        }

        GridInteraction.RemoveObject(flora.Tile);
        FloraMaster.Instance.Remove(flora);
    }

    public void OnUse(UsedContext context) { }
}
