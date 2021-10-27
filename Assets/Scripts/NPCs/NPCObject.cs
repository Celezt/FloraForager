using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// foundation for NPC
/// </summary>
public class NPCObject : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private string _NameID;
    [SerializeField]
    private float _ExitRadius = 5.0f;
    [SerializeField] 
    private LayerMask _LayerMasks;

    private GameObject _Player;

    public NPC NPC { get; private set; }
    public Bounds Bounds { get; private set; }

    public int Priority => 3;

    private void Awake()
    {
        Bounds = GetComponent<MeshFilter>().mesh.bounds;
        NPC = NPCManager.Instance.Get(_NameID.ToLower());
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !CanvasUtility.IsPointerOverUIElement();

        if (collision && hitInfo.transform.gameObject == gameObject)
        {
            NPCUI.Instance.SetActive(this, true);
        }
        else if ((collision && !hitInfo.transform.CompareTag("NPC")) || !collision)
        {
            NPCUI.Instance.SetActive(null, false);
        }

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

            DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue.Item1, dialogue.Item2).Completed += (DialogueManager manager) =>
            {
                playerInput.ActivateInput();
            };
        }
        else
        {
            (string, string[]) dialogue = NPC.RepeatingDialogue;

            if (!string.IsNullOrWhiteSpace(dialogue.Item1))
            {
                DialogueManager.GetByIndex(context.playerIndex).StartDialogue(dialogue.Item1, dialogue.Item2).Completed += (DialogueManager manager) =>
                {
                    OpenCommissionWindow();
                    playerInput.ActivateInput();
                };
            }
            else
            {
                OpenCommissionWindow();
            }
        }

        void OpenCommissionWindow()
        {
            if (NPC.HasCommissions)
            {
                CommissionGiverWindow.Instance.ShowCommissions(NPC);
                CommissionGiverWindow.Instance.Open();
            }
        }
    }
}
