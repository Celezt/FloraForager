using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerAction
{
    private List<PlayerActionHandle> _sharedDisable = new List<PlayerActionHandle>();

    public PlayerActionHandle AddSharedDisable()
    {
        PlayerActionHandle handle = PlayerActionHandle.Create(this);
        
        _sharedDisable.Add(handle);

        asset.Disable();
        
        return handle;
    }

    public void RemoveSharedDisable(PlayerActionHandle handle)
    {       
        _sharedDisable.Remove(handle);

        if (_sharedDisable.Count == 0)
            asset.Enable();
    }
}
