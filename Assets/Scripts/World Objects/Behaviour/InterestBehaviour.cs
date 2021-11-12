using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class InterestBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField, HideLabel, InlineProperty]
    private DialogueElement dialogue;

    public int Priority => 3;

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

        playerInput.DeactivateInput();

        void CompleteAction(DialogueManager manager)
        {
            playerInput.ActivateInput();
            manager.Completed -= CompleteAction;
        };

        DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue.Dialogue, dialogue.Aliases).Completed += CompleteAction;
    }
}
