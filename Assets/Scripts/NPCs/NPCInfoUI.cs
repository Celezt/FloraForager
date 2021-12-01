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
    [SerializeField] private float _MaxHitDistance = 10.0f;
    [SerializeField] private float _HeightOffset = 0.2f;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private NPCBehaviour _NPCObject;
    private Bounds _NPCBounds;

    private GameObject _Player;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = GetComponentInParent<Canvas>().rootCanvas;
        _CanvasRect = _Canvas.GetComponent<RectTransform>();

        _Player = PlayerInput.GetPlayerByIndex(0).gameObject;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !CanvasUtility.IsPointerOverUIElement();

        float distance = Mathf.Sqrt(
            Mathf.Pow(hitInfo.point.x - _Player.transform.position.x, 2) +
            Mathf.Pow(hitInfo.point.z - _Player.transform.position.z, 2));

        if (collision && distance <= _MaxHitDistance)
        {
            if (_NPCObject == null && hitInfo.transform.TryGetComponent(out NPCBehaviour npc))
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

    private void Show(NPCBehaviour npc)
    {
        _NPCObject = npc;

        if (npc.TryGetComponent(out Collider collider))
            _NPCBounds = collider.bounds;

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
            _NPCObject.transform.position + Vector3.up * (_NPCBounds.size.y + _HeightOffset));
    }

    private void UpdateWindow()
    {
        _Name.text = _NPCObject.NPC.Name;
        _Relation.text = _NPCObject.NPC.Relation.Relation.ToString();
    }
}
