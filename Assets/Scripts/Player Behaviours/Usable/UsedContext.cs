using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UsedContext
{
    public readonly IUse used;
    public readonly List<string> labels;
    public readonly UseBehaviour useBehaviour;
    public readonly Transform transform;
    public readonly string name;
    public readonly string itemTypeId;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal UsedContext(
        IUse used,
        List<string> labels,
        Transform transform,
        UseBehaviour useBehaviour,
        string name,
        string id,
        int playerIndex,
        bool canceled,
        bool started,
        bool performed)
    {
        this.used = used;
        this.labels = labels;
        this.transform = transform;
        this.useBehaviour = useBehaviour;
        this.name = name;
        this.itemTypeId = id;
        this.playerIndex = playerIndex;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }
}
