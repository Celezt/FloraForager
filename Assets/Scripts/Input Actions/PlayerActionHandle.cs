using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerActionHandle
{
    private int _id;

    public static PlayerActionHandle Create()
    {
        return new PlayerActionHandle { _id = Guid.NewGuid().GetHashCode() };
    }

    public override int GetHashCode() => _id;
}
