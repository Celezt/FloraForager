using System.Collections.Generic;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEngine;

public class HarvestScythe : IHarvest
{
    [ListDrawerSettings(ShowItemCount = false, DraggableItems = false, Expanded = true)]
    public DropType[] Rewards = new DropType[1] { new DropType { } };

    public void Initialize(FloraInfo data, IHarvest harvestData)
    {
        Rewards = (harvestData as HarvestScythe).Rewards;
    }

    public bool Harvest(UsedContext context, Flora flora)
    {
        if (flora.Completed)
        {
            GameObject heldObject = flora.Cell.HeldObject;

            Bounds bounds = new Bounds();
            if (heldObject.TryGetComponent(out MeshFilter meshFilter))
                bounds = meshFilter.mesh.bounds;

            context.Drop(heldObject.transform.position + Vector3.up * bounds.extents.y, Rewards);
            return true;
        }

        return false;
    }
}
