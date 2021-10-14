using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UseContext
{
    public readonly List<string> labels;
    public readonly UseBehaviour useBehaviour;
    public readonly Transform playerTransform;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly int slotIndex;
    public readonly int amount;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal UseContext(
        List<string> labels,
        Transform playerTransform,
        UseBehaviour useBehaviour,
        string name,
        string id,
        int playerIndex,
        int slotIndex,
        int amount,
        bool canceled,
        bool started,
        bool performed)
    {
        this.labels = labels;
        this.playerTransform = playerTransform;
        this.useBehaviour = useBehaviour;
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
        this.slotIndex = slotIndex;
        this.amount = amount;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }

    /// <summary>
    /// Consume the current item
    /// </summary>
    public void Consume(int amount = 1)
    {
        useBehaviour.ConsumeCurrentItem(amount);
    }
}
