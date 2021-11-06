using System.Collections.Generic;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class HarvestScythe : IHarvest
{
    [ListDrawerSettings(Expanded = true)]
    public DropType[] Rewards = new DropType[1] { new DropType { } };

    public void Initialize(FloraInfo data, IHarvest harvestData)
    {
        Rewards = (harvestData as HarvestScythe).Rewards;
    }

    public bool Harvest(UsedContext context, Flora flora)
    {
        if (flora.Completed)
        {
            context.Drop(flora.Cell.HeldObject.transform.position, Rewards);
            return true;
        }

        return false;
    }
}
