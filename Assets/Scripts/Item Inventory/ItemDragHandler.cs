using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup _canvasGroup;
    private Transform _dragParent;
    private Transform _startParent;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _dragParent = GetComponentInParent<InventoryManager>().transform;
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        _startParent = transform.parent;
        transform.SetParent(_dragParent.transform);
        transform.localPosition = Vector3.zero;
        _canvasGroup.alpha = 0.8f;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        
        transform.position = Mouse.current.position.ReadValue();
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;
        _canvasGroup.alpha = 1f;
        transform.SetParent(_startParent);
        transform.localPosition = Vector3.zero;        
    }

}
