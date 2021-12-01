using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// foundation for NPC
/// </summary>
public class NPCBehaviour : MonoBehaviour, IInteractable, IUsable
{
    [SerializeField]
    private HumanoidAnimationBehaviour _HumanoidAnimationBehaviour;
    [SerializeField]
    private AnimationClip _ScaredClip;
    [SerializeField] 
    private string _NameID;
    [SerializeField]
    private float _ExitRadius = 5.0f;
        
    private GameObject _Player;

    public NPC NPC { get; private set; }

    public int Priority => 3;

    private void Awake()
    {
        NPC = NPCManager.Instance.Get(_NameID.ToLower());
    }

    private void Start()
    {
        foreach (Commission commission in CommissionList.Instance.Commissions)
        {
            NPC.SetCommission(commission);
        }
    }

    private void Update()
    {
        if (CommissionGiverWindow.Instance.Opened && _Player != null)
        {
            if (Vector3.Distance(transform.position, _Player.transform.position) > _ExitRadius)
            {
                CommissionGiverWindow.Instance.Exit();
                _Player = null;
            }
        }
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);
        _Player = playerInput.gameObject;

        playerInput.DeactivateInput();

        if (NPC.DialogueQueue.Count > 0)
        {
            (string, string[]) dialogue = NPC.DialogueQueue.Dequeue();

            if (!string.IsNullOrWhiteSpace(dialogue.Item1))
            {
                void CompleteAction(DialogueManager manager)
                {
                    playerInput.ActivateInput();
                    manager.Completed -= CompleteAction;
                };

                DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue.Item1, dialogue.Item2).Completed += CompleteAction;
            }
            else
                playerInput.ActivateInput();
        }
        else
        {
            (string, string[]) dialogue = NPC.RepeatingDialogue;

            if (!string.IsNullOrWhiteSpace(dialogue.Item1))
            {
                void CompleteAction(DialogueManager manager)
                {
                    playerInput.ActivateInput();

                    if (!CommissionGiverWindow.Instance.Opened)
                        NPC.OpenCommissions();

                    manager.Completed -= CompleteAction;
                };

                DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue.Item1, dialogue.Item2).Completed += CompleteAction;
            }
            else
                NPC.OpenCommissions();
        }
    }

    ItemLabels IUsable.Filter() => ItemLabels.Axe | ItemLabels.Scythe | ItemLabels.Pickaxe;

    void IUsable.OnUse(UsedContext context)
    {
        if (_ScaredClip == null)
            return;

        _HumanoidAnimationBehaviour.CustomMotionRaise(_ScaredClip);
    }
}
