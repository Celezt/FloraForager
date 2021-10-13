using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class DebugDialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private AssetReferenceText _asset;

    public int Priority => 10;

    public void OnInteract(InteractContext context)
    {
        if (context.started)
        {
            PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

            playerInput.DeactivateInput();

            DialogueManager.GetByIndex(context.playerIndex).StartDialogue(_asset, "You", "Joker").Completed += (handle) => 
            {
                playerInput.ActivateInput();
            };
        }
    }
}
