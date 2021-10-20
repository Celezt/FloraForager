using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemTypeContext
{
    public readonly Transform transform;
    public readonly UseBehaviour useBehaviour;
    public readonly PlayerInfo playerInfo;
    public readonly int playerIndex;
    public readonly int slotIndex;

    internal ItemTypeContext(
        Transform transform,
        UseBehaviour useBehaviour,
        PlayerInfo playerInfo,
        int playerIndex,
        int slotIndex)
    {
        this.transform = transform;
        this.useBehaviour = useBehaviour;
        this.playerInfo = playerInfo;
        this.playerIndex = playerIndex;
        this.slotIndex = slotIndex;
    }
}
