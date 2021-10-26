using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemTypeContext
{
    public readonly Transform transform;
    public readonly UseBehaviour behaviour;
    public readonly PlayerInfo playerInfo;
    public readonly int playerIndex;
    public readonly int slotIndex;

    internal ItemTypeContext(
        Transform transform,
        UseBehaviour behaviour,
        PlayerInfo playerInfo,
        int playerIndex,
        int slotIndex)
    {
        this.transform = transform;
        this.behaviour = behaviour;
        this.playerInfo = playerInfo;
        this.playerIndex = playerIndex;
        this.slotIndex = slotIndex;
    }
}
