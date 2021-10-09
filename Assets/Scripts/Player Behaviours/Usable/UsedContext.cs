using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UsedContext
{
    public readonly IUse used;
    public readonly List<string> labels;
    public readonly UseBehaviour useBehaviour;
    public readonly Transform playerTransform;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal UsedContext(
        IUse used,
        List<string> labels,
        Transform playerTransform,
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
        this.playerTransform = playerTransform;
        this.useBehaviour = useBehaviour;
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }
}
