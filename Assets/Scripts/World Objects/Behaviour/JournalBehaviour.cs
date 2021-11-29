using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JournalBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string _JournalID;
    [SerializeField]
    private string _PickupSound = "pickup_04";

    public int Priority => 2;

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        Inventory inventory = PlayerInput.GetPlayerByIndex(context.playerIndex).GetComponent<PlayerInfo>().Inventory;

        if (inventory.Insert(_JournalID, 1))
        {
            SoundPlayer.Instance.Play(_PickupSound);
            gameObject.SetActive(false);
        }
    }
}
