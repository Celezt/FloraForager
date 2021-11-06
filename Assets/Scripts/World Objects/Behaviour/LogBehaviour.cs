using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public class LogBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string _LogID;

    public int Priority => 2;

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        Inventory inventory = PlayerInput.GetPlayerByIndex(context.playerIndex).GetComponent<PlayerInfo>().Inventory;

        if (inventory.Insert(_LogID, 1))
            Destroy(gameObject);
    }
}
