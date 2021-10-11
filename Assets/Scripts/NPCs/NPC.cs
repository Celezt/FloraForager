using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// foundation for NPC
/// </summary>
public class NPC : MonoBehaviour
{
    [SerializeField] private LayerMask _LayerMasks;

    private RelationshipManager _Relations;
    private Bounds _Bounds;

    public RelationshipManager Relations => _Relations;
    public Bounds Bounds => _Bounds;

    private void Awake()
    {
        _Relations = GetComponent<RelationshipManager>();
        _Bounds = GetComponent<MeshFilter>().mesh.bounds;
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
}
