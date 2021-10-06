using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class NPCUI : Singleton<NPCUI>
{
    [SerializeField] private Text _Name;
    [SerializeField] private Text _Relation;
    [SerializeField] private float _HeightOffset = 2.0f;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private NPC _NPC;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = transform.parent.GetComponent<Canvas>();
        _CanvasRect = transform.parent.GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    public void SetActive(NPC npc, bool value)
    {
        _NPC = npc;

        UpdateText();
        _CanvasGroup.alpha = value ? 1.0f : 0.0f;
    }

    private void UpdatePosition()
    {
        if (_NPC == null)
            return;

        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _NPC.transform.position + Vector3.up * _NPC.Bounds.size.y * _HeightOffset);
    }

    public void UpdateText()
    {
        if (_NPC == null)
            return;

        _Name.text = _NPC.name;
        _Relation.text = _NPC.Relations.Relation.ToString();
    }
}
