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

    /// <summary>
    /// If this NPC is currently selected
    /// </summary>
    public bool Selected { get; private set; }

    public int Priority => 2;

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !EventSystem.current.IsPointerOverGameObject();

        Selected = collision && hitInfo.transform.gameObject == gameObject;
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;


    }
}
