using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerActionHandle
{
    public bool IsEmpty => _id == 0;

    private int _id;

    public void Clear() => _id = 0;

    public static PlayerActionHandle Create()
    {
        return new PlayerActionHandle { _id = Guid.NewGuid().GetHashCode() };
    }

    public override int GetHashCode() => _id;
}
