using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemContext
{
    public readonly List<string> labels;
    public readonly UseBehaviour useBehaviour;
    public readonly Transform transform;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly int slotIndex;
    public readonly int amount;

    internal ItemContext(
        List<string> labels,
        Transform transform,
        UseBehaviour useBehaviour,
        string name,
        string id,
        int playerIndex,
        int slotIndex,
        int amount)
    {
        this.labels = labels;
        this.transform = transform;
        this.useBehaviour = useBehaviour;
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
        this.slotIndex = slotIndex;
        this.amount = amount;
    }
}
