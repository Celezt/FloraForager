using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// foundation for NPC
/// </summary>
public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private LayerMask _LayerMasks;

    private RelationshipManager _Relations;
    private Bounds _Bounds;

    /// <summary>
    /// If this NPC is currently selected
    /// </summary>
    public bool Selected { get; private set; }

    public RelationshipManager Relations => _Relations;
    public Bounds Bounds => _Bounds;

    public int Priority => 2;

    private void Awake()
    {
        _Relations = GetComponent<RelationshipManager>();
        _Bounds = GetComponent<MeshFilter>().mesh.bounds;
    }

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !EventSystem.current.IsPointerOverGameObject();

        Selected = collision && hitInfo.transform.gameObject == gameObject;

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


    }
}
