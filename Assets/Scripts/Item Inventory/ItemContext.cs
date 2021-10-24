using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemContext
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

    internal ItemContext(
        List<string> labels,
        Transform transform,
        UseBehaviour behaviour,
        PlayerInfo playerInfo,
        string name,
        string id,
        int playerIndex,
        int slotIndex,
        int amount)
    {
        this.labels = labels;
        this.transform = transform;
        this.behaviour = behaviour;
        this.playerInfo = playerInfo;
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
        this.slotIndex = slotIndex;
        this.amount = amount;
    }
}
