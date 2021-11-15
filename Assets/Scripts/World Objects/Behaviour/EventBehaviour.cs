using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class EventBehaviour : MonoBehaviour, IStreamable<EventBehaviour.Data>
{
    [SerializeField, LabelText("Destroy When Invoked")]
    private bool _SaveIfInvoked = true;

    [PropertySpace(5)]
    public UnityEvent Events;
    [SerializeField, LabelText("Dialogue to add")]
    [ListDrawerSettings(DraggableItems = false, ShowItemCount = false, Expanded = true)]
    private DialogueEvent[] _Dialogue;

    private Data _Data;

    public class Data
    {
        public bool IsInvoked = false;
    }
    public Data OnUpload() => _Data = new Data();
    public void OnLoad(object state)
    {
        _Data = state as Data;

        if (_Data.IsInvoked)
            Destroy(this);
    }
    public void OnBeforeSaving()
    {

    }

    public void Invoke()
    {
        Events.Invoke();
        AddDialogue();

        Destroy(this);
    }

    private void AddDialogue()
    {
        if (_Dialogue == null)
        {
            Debug.LogWarning("dialogue is null, either has already been invoked or was initially empty");
            return;
        }

        // add all dialogue
        //
        foreach (DialogueEvent dialogueEvent in _Dialogue)
        {
            foreach (DialoguePriority dialouge in dialogueEvent.AddedDialogue)
            {
                NPCManager.Instance.EnqueueDialogue(dialogueEvent.NPC, dialouge.Asset, dialouge.Priority, dialouge.Aliases);
            }
        }
    }

    private void OnDestroy()
    {
        if (LoadScene.SceneIsLoading)
            return;

        if (_SaveIfInvoked && _Data != null)
            _Data.IsInvoked = true;
    }
}
