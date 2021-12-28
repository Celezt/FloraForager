using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;

public class InterestBehaviour : MonoBehaviour, IStreamable<InterestBehaviour.Data>, IInteractable
{
    [SerializeField, ListDrawerSettings(ShowItemCount = false, DraggableItems = false, Expanded = true)]
    private AssetReferenceText[] _DialogueQueue;
    [Title("Repeatable")]
    [SerializeField, HideLabel, InlineProperty]
    private AssetReferenceText _repeatable;

    private Data _data;

    public class Data
    {
        public Queue<string> Dialogue;
    }

    public Data OnUpload() => _data;
    public void OnLoad(object state)
    {
        _data = state as Data;
    }
    public void OnBeforeSaving()
    {

    }

    public int Priority => 3;

    private void Awake()
    {
        _data = new Data() 
        { 
            Dialogue = new Queue<string>(System.Array.ConvertAll(_DialogueQueue, d => d.AssetGUID)) 
        };
    }

    private void Start()
    {
        if (_DialogueQueue.Length <= 0 && string.IsNullOrEmpty(_repeatable.AssetGUID))
            Destroy(this);
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        string dialogueAsset = GetDialogue();
        
        if (_DialogueQueue.Length <= 0 && string.IsNullOrEmpty(_repeatable.AssetGUID))
            Destroy(this);

        if (string.IsNullOrWhiteSpace(dialogueAsset))
            return;

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

        playerInput.DeactivateInput();

        void CompleteAction(DialogueManager manager)
        {
            playerInput.ActivateInput();
            manager.Completed -= CompleteAction;
        };

        DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogueAsset).Completed += CompleteAction;
    }

    private string GetDialogue()
    {
        if (_data.Dialogue.Count > 0)
            return _data.Dialogue.Dequeue();
            
        return _repeatable.AssetGUID;
    }
}
