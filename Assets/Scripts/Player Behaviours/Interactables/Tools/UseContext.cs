using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UseContext
{
    public readonly Transform playerTransform;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal UseContext(
        Transform playerTransform,
        int playerIndex,
        bool canceled,
        bool started,
        bool performed)
    {
        this.playerTransform = playerTransform;
        this.playerIndex = playerIndex;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }
}
