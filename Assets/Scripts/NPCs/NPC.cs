using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// foundation for NPC
/// </summary>
public class NPC : MonoBehaviour
{
    /// <summary>
    /// If this NPC is currently selected
    /// </summary>
    public bool Selected { get; private set; }

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("NPC"));

        Selected = collision && hitInfo.transform.parent != null && hitInfo.transform.parent.gameObject == gameObject;
    }
}
