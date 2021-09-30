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
            PlayerMovement movement = playerInput.GetComponent<PlayerMovement>();
            InteractBehaviour interactableArea = playerInput.GetComponent<InteractBehaviour>();
            PlayerActionHandle playerHandle1 = movement.Inputs.AddSharedDisable();
            PlayerActionHandle playerHandle2 = interactableArea.Inputs.AddSharedDisable();

            DialogueManager.GetDialogueByIndex(context.playerIndex).StartDialogue(_asset, "You", "Joker").Completed += (handle) => 
            {
                movement.Inputs.RemoveSharedDisable(playerHandle1);
                interactableArea.Inputs.RemoveSharedDisable(playerHandle2);
            };
        }
    }
}
