using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UseContext
{
    public readonly IUse used;
    public readonly List<string> labels;
    public readonly Transform playerTransform;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal UseContext(
        IUse used,
        List<string> labels,
        Transform playerTransform,
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
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }
}
