using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject _Inventory;
    [SerializeField]
    private TMP_Text _ItemName;
    [SerializeField]
    private float _Offset = 0.1f;

    private GraphicRaycaster _Raycaster;
    private EventSystem _EventSystem;

    private CanvasGroup _InventoryCG;
    private CanvasGroup _ItemInfoCG;

    private void Start()
    {
        _EventSystem = EventSystem.current;
        _Raycaster = GetComponentInParent<GraphicRaycaster>();

        _InventoryCG = _Inventory.GetComponent<CanvasGroup>();
        _ItemInfoCG = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_InventoryCG.alpha <= float.Epsilon)
        {
            _ItemInfoCG.alpha = 0.0f;
            return;
        }

        ItemSlot itemSlot = null;

        PointerEventData pointerEventData = new PointerEventData(_EventSystem);
        pointerEventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();

        _Raycaster.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            itemSlot = result.gameObject.GetComponent<ItemSlot>();

            if (itemSlot != null)
                break;
        }

        if (itemSlot != null && !string.IsNullOrEmpty(itemSlot.Name))
        {
            _ItemInfoCG.alpha = 1.0f;

            _ItemName.text = itemSlot.Name;
            transform.position = pointerEventData.position + Vector2.up * _Offset;
        }
        else
            _ItemInfoCG.alpha = 0.0f;
    }
}