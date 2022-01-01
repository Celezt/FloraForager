using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[RequireComponent(typeof(StreamableBehaviour))]
public class EventBehaviour : MonoBehaviour, IStreamable<EventBehaviour.Data>
{
    [SerializeField] private bool _saveIfInvoked = true;
    [SerializeField] private bool _isTrigger = false;
    [SerializeField] private bool _invokeOnStart = false;

    [PropertySpace(5)]
    public UnityEvent Events;
    [SerializeField, LabelText("Dialogue To Add")]
    [ListDrawerSettings(DraggableItems = false, ShowItemCount = false, Expanded = true)]
    private DialogueEvent[] _dialogue = null;

    private Data _data;

    public class Data
    {
        public bool IsInvoked = false;
    }
    public Data OnUpload() => _data;
    public void OnLoad(object state)
    {
        _data = state as Data;

        if (_data.IsInvoked)
            Destroy(this);
    }
    public void OnBeforeSaving()
    {

    }

    private void Awake()
    {
        _data = new Data();
    }

    private void Start()
    {
        if (_invokeOnStart)
            Invoke();
    }

    public void Invoke()
    {
        Events.Invoke();
        AddDialogue();

        Destroy(this);
    }

    private void AddDialogue()
    {
        if (_dialogue == null)
            return;

        // add all dialogue
        //
        foreach (DialogueEvent dialogueEvent in _dialogue)
        {
            foreach (DialoguePriority dialouge in dialogueEvent.AddedDialogue)
            {
                NPCManager.Instance.EnqueueDialogue(dialogueEvent.NPC, dialouge.Dialogue, dialouge.Priority);
            }
        }
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            if (_saveIfInvoked && _data != null)
                _data.IsInvoked = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isTrigger && other.CompareTag("Player"))
            Invoke();
    }
}
