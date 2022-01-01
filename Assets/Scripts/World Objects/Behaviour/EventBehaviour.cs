using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using MyBox;

[RequireComponent(typeof(StreamableBehaviour))]
public class EventBehaviour : MonoBehaviour, IStreamable<EventBehaviour.Data>
{
    [SerializeField] private bool _saveIfInvoked = true;
    [SerializeField] private bool _invokeOnStart = false;

    [PropertySpace(5)]
    public UnityEvent EventsBeforeDestroy;
    public UnityEvent EventsOnInvoke;
    [SerializeField, LabelText("Dialogue To Add")]
    [ListDrawerSettings(DraggableItems = false, ShowItemCount = false, Expanded = true)]
    private DialogueEvent[] _dialogue = null;

    private Data _data;
    private System.Guid _guid;

    public class Data
    {
        public bool IsInvoked = false;
    }
    public Data OnUpload() => _data;
    public void OnLoad(object state)
    {
        _data = state as Data;

        if (_data.IsInvoked)
            Destroy();
    }
    public void OnBeforeSaving()
    {

    }

    private void Awake()
    {
        _data = new Data();
        _guid = GetComponent<GuidComponent>().Guid;
    }

    private void Start()
    {
        if (GameManager.Stream.StreamedData.ContainsKey(_guid))
        {
            if (!_data.IsInvoked && _invokeOnStart)
                Invoke();
            else
                Destroy();
        }
    }

    public void Invoke()
    {
        EventsOnInvoke.Invoke();
        AddDialogue();

        if (_saveIfInvoked)
            _data.IsInvoked = true;

        Destroy(this);
    }

    public void Destroy()
    {
        EventsBeforeDestroy.Invoke();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Invoke();
    }
}
