using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DebugDialogueInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private AssetReferenceText _asset;

    public int Priority => 10;

    public void OnInteract(InteractContext context)
    {
        if (context.started)
        {
            DialogueManager.Instance.StartDialogue(_asset);
        }
    }
}
