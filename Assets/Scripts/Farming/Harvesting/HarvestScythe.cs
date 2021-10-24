using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HarvestScythe : IHarvest
{
    public void Initialize(FloraInfo data, IHarvest harvestData)
    {

    }

    public bool Harvest(Flora flora, int playerIndex)
    {
        if (flora.Completed)
        {
            Inventory inventory = PlayerInput.GetPlayerByIndex(playerIndex).GetComponent<PlayerInfo>().Inventory;

            bool canHarvest = true;
            foreach (RewardPair reward in flora.FloraInfo.Rewards)
            {
                int emptySpace = inventory.FindEmptySpace(reward.ItemID);

                if (emptySpace < reward.Amount)
                    canHarvest = false;
            }

            if (!canHarvest)
                return false;

            foreach (RewardPair reward in flora.FloraInfo.Rewards)
            {
                inventory.Insert(new ItemAsset
                {
                    ID = reward.ItemID,
                    Amount = reward.Amount
                });
            }
        }

        return true;
    }
}
