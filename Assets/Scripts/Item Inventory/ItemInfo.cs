using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject _Inventory;
    [SerializeField]
    private TMP_Text _ItemName;
    [SerializeField]
    private TMP_Text _ItemLabels;
    [SerializeField] 
    private Image _ItemStarsImage;

    [SerializeField] private SpriteAtlas _StarsAtlas;

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

        if (itemSlot != null && !string.IsNullOrEmpty(itemSlot.Item.ID) && 
            ItemTypeSettings.Instance.ItemNameChunk.TryGetValue(itemSlot.Item.ID, out string name))
        {
            _ItemInfoCG.alpha = 1.0f;

            if (_ItemName.text != name)
            {
                _ItemName.text = name;
                _ItemLabels.text = string.Empty;

                if (ItemTypeSettings.Instance.ItemLabelChunk.TryGetValue(itemSlot.Item.ID, out List<string> labels))
                {
                    foreach (string label in labels)
                    {
                        _ItemLabels.text += string.Join(" ", Regex.Split(label, @"(?<!^)(?=[A-Z])"));

                        if (label != labels[labels.Count - 1])
                            _ItemLabels.text += " | ";
                    }
                }
                if (ItemTypeSettings.Instance.ItemTypeChunk.TryGetValue(itemSlot.Item.ID, out ItemType itemType))
                {
                    Stars? star = (itemType.Behaviour as IStar)?.Star;

                    int index = star.HasValue ? (int)star : 0;
                    Sprite starSprite = _StarsAtlas.GetSprite($"stars_{index}");

                    _ItemStarsImage.sprite = starSprite;
                    _ItemStarsImage.enabled = index > 0;
                }
            }

            transform.position = pointerEventData.position;
        }
        else
            _ItemInfoCG.alpha = 0.0f;
    }
}