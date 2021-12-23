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
    private AnimationBehaviour _HumanoidAnimationBehaviour;
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

    private event System.Action OnRotate = delegate { };

    public NPC NPC { get; private set; }

    public int Priority => 3;
    public bool OpenCommissions = false;

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
                StartDialogue(context.playerIndex, dialogue);

                void CompleteAction(DialogueManager manager)
                {
                    if (!CommissionGiverWindow.Instance.Opened && OpenCommissions)
                        NPC.OpenCommissions();

                    OpenCommissions = false;

                    manager.Completed -= CompleteAction;
                };

                DialogueManager.GetByIndex(context.playerIndex).Completed += CompleteAction;
            }
            else
                playerInput.ActivateInput();
        }
        else
        {
            string dialogue = NPC.RepeatingDialogue;

            if (!string.IsNullOrWhiteSpace(dialogue))
            {
                StartDialogue(context.playerIndex, dialogue);

                void CompleteAction(DialogueManager manager)
                {
                    if (!CommissionGiverWindow.Instance.Opened)
                        NPC.OpenCommissions();

                    manager.Completed -= CompleteAction;
                };

                DialogueManager.GetByIndex(context.playerIndex).Completed += CompleteAction;
            }
            else
            {
                NPC.OpenCommissions();
                playerInput.ActivateInput();
            }
        }
    }

    ItemLabels IUsable.Filter() => ItemLabels.Axe | ItemLabels.Scythe | ItemLabels.Pickaxe;

    void IUsable.OnUse(UsedContext context)
    {
        if (_ScaredClip == null)
            return;

        _HumanoidAnimationBehaviour.Play(_ScaredClip);
    }

    public void StartDialogue(int playerIndex, string assetGUID)
    {
        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(playerIndex);
        playerInput.DeactivateInput();

        PlayerMovement playerMovement = playerInput.GetComponent<PlayerMovement>();
        UseBehaviour playerUse = playerInput.GetComponent<UseBehaviour>();

        UIStateVisibility.Instance.Hide("player_hud", "inventory");

        if (_RotateBack != null)
            StopCoroutine(_RotateBack);

        _RotateTowards = StartCoroutine(RotateTowardsTarget(playerInput.gameObject));
        playerMovement.RotateTowardsTarget(gameObject);

        OnRotate += OnRotateAction;

        void OnRotateAction()
        {
            void CompleteAction(DialogueManager manager)
            {
                if (_RotateTowards != null)
                    StopCoroutine(_RotateTowards);
                if (playerMovement.RotateTowards != null)
                    playerMovement.StopCoroutine(playerMovement.RotateTowards);

                _RotateBack = StartCoroutine(RotateBack());

                playerUse.ApplyCooldown();

                playerInput.ActivateInput();

                manager.Completed -= CompleteAction;
            };

            DialogueManager.GetByIndex(playerIndex).StartDialogue(assetGUID).Completed += CompleteAction;

            OnRotate -= OnRotateAction;
        };
    }

    private IEnumerator RotateTowardsTarget(GameObject target, float timeToRotate = 0.75f)
    {
        Quaternion startRotation = transform.rotation;
        Vector3 direction = target.transform.position - transform.position;
        Quaternion finalRotation = Quaternion.LookRotation(direction);

        Quaternion start = new Quaternion(0, startRotation.y, 0, startRotation.w);
        Quaternion final = new Quaternion(0, finalRotation.y, 0, finalRotation.w);

        float walkSpeed = 0.06f;

        _HumanoidAnimationBehaviour.WalkSpeed = walkSpeed;

        float time = 0.0f;
        while (time <= 1f)
        {
            direction = target.transform.position - transform.position;
            finalRotation = Quaternion.LookRotation(direction);

            final = new Quaternion(0, finalRotation.y, 0, finalRotation.w);

            time += Time.deltaTime / timeToRotate;
            transform.rotation = Quaternion.Slerp(start, final, time);

            yield return null;
        }

        OnRotate.Invoke();

        time = 0.0f;
        while (time <= 1f)
        {
            time += Time.deltaTime / 0.5f;
            _HumanoidAnimationBehaviour.WalkSpeed = Mathf.Lerp(walkSpeed, 0.0f, time);
            yield return null;
        }

        transform.rotation = final;
        _RotateTowards = null;
    }
    private IEnumerator RotateBack(float timeToRotate = 1.0f)
    {
        Quaternion startRotation = transform.rotation;

        Quaternion start = new Quaternion(0, startRotation.y, 0, startRotation.w);
        Quaternion final = new Quaternion(0, _startRotation.y, 0, _startRotation.w);

        float walkSpeed = 0.05f;

        _HumanoidAnimationBehaviour.WalkSpeed = walkSpeed;

        float time = 0.0f;
        while (time <= 1f)
        {
            time += Time.deltaTime / timeToRotate;
            transform.rotation = Quaternion.Slerp(start, final, time);
            yield return null;
        }

        time = 0.0f;
        while (time <= 1f)
        {
            time += Time.deltaTime / 0.5f;
            _HumanoidAnimationBehaviour.WalkSpeed = Mathf.Lerp(walkSpeed, 0.0f, time);
            yield return null;
        }

        transform.rotation = final;
        _RotateBack = null;
    }
}
