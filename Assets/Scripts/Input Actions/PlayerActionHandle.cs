using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerActionHandle
{
    public bool IsEmpty => _id == 0;

    private int _id;
    private PlayerAction _playerActionReference;

    public void Clear() => _id = 0;

    public static PlayerActionHandle Create(PlayerAction reference)
    {
        return new PlayerActionHandle { _id = Guid.NewGuid().GetHashCode(), _playerActionReference = reference };
    }

    public override int GetHashCode() => _id;

    public void RemoveSharedDisable()
    {
        _playerActionReference.RemoveSharedDisable(this);
    }
}
