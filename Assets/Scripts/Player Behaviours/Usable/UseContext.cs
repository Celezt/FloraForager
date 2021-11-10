using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UseContext
{
    public readonly List<string> labels;
    public readonly Transform transform;
    public readonly UseBehaviour behaviour;
    public readonly PlayerInfo playerInfo;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly int slotIndex;
    public readonly int amount;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    private readonly ItemType _itemType;

    internal UseContext(
        ItemType item,
        Transform transform,
        UseBehaviour behaviour,
        PlayerInfo playerInfo,
        int playerIndex,
        int slotIndex,
        int amount,
        bool canceled,
        bool started,
        bool performed)
    {
        _itemType = item;
        this.labels = _itemType.Labels;
        this.transform = transform;
        this.behaviour = behaviour;
        this.playerInfo = playerInfo;
        this.name = _itemType.Name;
        this.id = _itemType.ID;
        this.playerIndex = playerIndex;
        this.slotIndex = slotIndex;
        this.amount = amount;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }

    /// <summary>
    /// Create an instance and place it.
    /// </summary>
    /// <returns>If it was successfully placed.</returns>
    public bool Place(GameObject original, Vector3 position, LayerMask placeMask, bool followNormal = false) => Place(original, position, Quaternion.identity, placeMask, followNormal);
    /// <summary>
    /// Create an instance and place it.
    /// </summary>
    /// <returns>If it was successfully placed.</returns>
    public bool Place(GameObject original, Vector3 position, Quaternion rotation, LayerMask placeMask, bool followNormal = false)
    {
        if (!Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, placeMask))
            return false;

        GameObject obj = Object.Instantiate(original, hit.point, followNormal ? Quaternion.LookRotation(Vector3.forward, hit.normal) * rotation : rotation);

        IPlaceable[] placeables = obj.GetComponentsInChildren<IPlaceable>();

        if (placeables != null)
        {
            for (int i = 0; i < placeables.Length; i++)
            {
                placeables[i].OnPlace(new PlacedContext
                    (
                        _itemType as IUse,
                        _itemType.Labels,
                        transform,
                        behaviour,
                        name,
                        id,
                        playerIndex,
                        original.name
                    ));
            }
        }

        return true;
    }

    public void Place(GameObject original, Cell cell, Quaternion rotation, bool followNormal = false)
    {
        GameObject obj = Object.Instantiate(original, cell.Middle, followNormal ? Quaternion.LookRotation(Vector3.forward, Vector3.up) * rotation : rotation);
        
        cell.Occupy(obj);

        IPlaceable[] placeables = obj.GetComponentsInChildren<IPlaceable>();

        if (placeables != null)
        {
            for (int i = 0; i < placeables.Length; i++)
            {
                placeables[i].OnPlace(new PlacedContext
                    (
                        _itemType as IUse,
                        _itemType.Labels,
                        transform,
                        behaviour,
                        name,
                        id,
                        playerIndex,
                        original.name
                    ));
            }
        }
    }

    /// <summary>
    /// Consume the current item.
    /// </summary>
    public void Consume(int amount = 1)
    {
        behaviour.ConsumeCurrentItem(amount);
    }

    /// <summary>
    /// Call object with usable.
    /// </summary>
    public void CallUsable(IUsable usable)
    {
        behaviour.CallUsable(this, usable);
    }
}
