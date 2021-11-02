using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using MyBox;

public class NPCInfoUI : MonoBehaviour
{
    [SerializeField] private LayerMask _LayerMasks;
    [SerializeField] private TMP_Text _Name;
    [SerializeField] private TMP_Text _Relation;
    [SerializeField] private float _HeightOffset = 2.0f;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private NPCObject _NPCObject;
    private Bounds _NPCBounds;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = transform.root.GetComponent<Canvas>();
        _CanvasRect = _Canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !CanvasUtility.IsPointerOverUIElement();

        if (collision)
        {
            if (_NPCObject == null && hitInfo.transform.TryGetComponent(out NPCObject npc))
            {
                Show(npc);
            }
        }
        else
            Hide();
    }

    private void LateUpdate()
    {
        if (_NPCObject == null)
            return;

        UpdatePosition();
    }

    private void Show(NPCObject npc)
    {
        _NPCObject = npc;
        _NPCBounds = npc.GetComponent<MeshFilter>().mesh.bounds;

        UpdateWindow();

        _CanvasGroup.alpha = 1.0f;
    }

    private void Hide()
    {
        _NPCObject = null;
        _CanvasGroup.alpha = 0.0f;
    }

    private void UpdatePosition()
    {
        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _NPCObject.transform.position + Vector3.up * _NPCBounds.size.y * _HeightOffset);
    }

    private void UpdateWindow()
    {
        _Name.text = _NPCObject.NPC.Name;
        _Relation.text = _NPCObject.NPC.Relation.Relation.ToString();
    }
}
