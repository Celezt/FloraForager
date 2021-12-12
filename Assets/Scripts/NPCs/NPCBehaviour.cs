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

    private Quaternion _startRotation;
        
    private GameObject _Player;

    private Coroutine _RotateTowards;
    private Coroutine _RotateBack;

    public NPC NPC { get; private set; }

    public int Priority => 3;

    private void Awake()
    {
        NPC = NPCManager.Instance.Get(_NameID);
        NPCManager.Instance.AddObject(_NameID, this);
    }

    private void Start()
    {
        _startRotation = transform.rotation;

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
            string dialogue = NPC.DialogueQueue.Dequeue();

            if (!string.IsNullOrWhiteSpace(dialogue))
            {
                if (_RotateBack != null)
                    StopCoroutine(_RotateBack);

                _RotateTowards = StartCoroutine(RotateTowardsTarget(playerInput.gameObject));

                void CompleteAction(DialogueManager manager)
                {
                    if (_RotateTowards != null)
                        StopCoroutine(_RotateTowards);

                    _RotateBack = StartCoroutine(RotateBack());

                    playerInput.ActivateInput();

                    manager.Completed -= CompleteAction;
                };

                DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue).Completed += CompleteAction;


            }
            else
                playerInput.ActivateInput();
        }
        else
        {
            string dialogue = NPC.RepeatingDialogue;

            if (!string.IsNullOrWhiteSpace(dialogue))
            {
                if (_RotateBack != null)
                    StopCoroutine(_RotateBack);

                _RotateTowards = StartCoroutine(RotateTowardsTarget(playerInput.gameObject));

                void CompleteAction(DialogueManager manager)
                {
                    if (_RotateTowards != null)
                        StopCoroutine(_RotateTowards);

                    _RotateBack = StartCoroutine(RotateBack());

                    if (!CommissionGiverWindow.Instance.Opened)
                        NPC.OpenCommissions();

                    playerInput.ActivateInput();

                    manager.Completed -= CompleteAction;
                };

                DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue).Completed += CompleteAction;
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

    private IEnumerator RotateTowardsTarget(GameObject target, float timeToRotate = 1.0f)
    {
        Quaternion startRotation = transform.rotation;
        Vector3 direction = target.transform.position - transform.position;
        Quaternion finalRotation = Quaternion.LookRotation(direction);

        Quaternion start = new Quaternion(0, startRotation.y, 0, startRotation.w);
        Quaternion final = new Quaternion(0, finalRotation.y, 0, finalRotation.w);

        _HumanoidAnimationBehaviour.WalkSpeed = 0.05f;

        float time = 0.0f;
        while (time <= 1f)
        {
            time += Time.deltaTime / timeToRotate;
            transform.rotation = Quaternion.Slerp(start, final, time);
            yield return null;
        }

        _HumanoidAnimationBehaviour.WalkSpeed = 0f;
        transform.rotation = final;
        _RotateTowards = null;
    }
    private IEnumerator RotateBack(float timeToRotate = 1.0f)
    {
        Quaternion startRotation = transform.rotation;

        Quaternion start = new Quaternion(0, startRotation.y, 0, startRotation.w);
        Quaternion final = new Quaternion(0, _startRotation.y, 0, _startRotation.w);

        _HumanoidAnimationBehaviour.WalkSpeed = 0.05f;

        float time = 0.0f;
        while (time <= 1f)
        {
            time += Time.deltaTime / timeToRotate;
            transform.rotation = Quaternion.Slerp(start, final, time);
            yield return null;
        }

        _HumanoidAnimationBehaviour.WalkSpeed = 0f;
        transform.rotation = final;
        _RotateBack = null;
    }
}
