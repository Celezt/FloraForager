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
    private NPCData _NPCData;
    [SerializeField] 
    private LayerMask _LayerMasks;

    public NPC NPC { get; private set; }
    public Bounds Bounds { get; private set; }

    public int Priority => 3;

    private void Awake()
    {
        NPC = NPCManager.Instance.Exists(_NPCData.Name) ?
            NPCManager.Instance.Get(_NPCData.Name) : 
            NPCManager.Instance.Add(_NPCData.Name, new NPC(_NPCData));

        Bounds = GetComponent<MeshFilter>().mesh.bounds;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !EventSystem.current.IsPointerOverGameObject();

        if (collision && hitInfo.transform.gameObject == gameObject)
        {
            NPCUI.Instance.SetActive(this, true);
        }
        else if ((collision && !hitInfo.transform.CompareTag("NPC")) || !collision)
        {
            NPCUI.Instance.SetActive(null, false);
        }
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        CommissionGiverWindow.Instance.ShowCommissions(NPC);
        CommissionGiverWindow.Instance.Open();
    }
}
