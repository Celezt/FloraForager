using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class UICraftingMenu : Singleton<UICraftingMenu>
{
    [SerializeField] private GameObject _CraftableItemPrefab;
    [SerializeField] private Transform _CraftingListArea;

    [SerializeField] private GameObject _Description;
    [SerializeField] private Image _DescriptionImage;
    [SerializeField] private Text _DescriptionText;

    private List<GameObject> _CraftableItemObjects;

    private CraftableItem _SelectedItem;
    private Workbench _Workbench;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    public Workbench Workbench => _Workbench;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = transform.parent.GetComponent<Canvas>();
        _CanvasRect = transform.parent.GetComponent<RectTransform>();

        _CraftableItemObjects = new List<GameObject>();
    }

    private void LateUpdate()
    {
        if (_Workbench == null)
            return;

        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _Workbench.transform.position + Vector3.up * 3.0f) + Vector2.left * 30.0f;
    }

    public void CraftItem()
    {
        if (_SelectedItem == null || !CanCraft(_SelectedItem))
            return;

        string itemID = string.Empty;

        foreach (ResourceRequirement<string, int> resReq in _SelectedItem.ResourceReqs)
        {
            itemID = resReq.ItemID;
            int amount = resReq.Amount;

            // remove amount of items from itemID
        }

        itemID = _SelectedItem.ItemID; // add this item to player's inventory


    }

    public void ShowCraftableItems(Workbench workbench)
    {
        _Workbench = workbench;

        _CraftableItemObjects.ForEach(o => Destroy(o));
        _CraftableItemObjects.Clear();

        _Description.SetActive(false);
        _SelectedItem = null;

        foreach (CraftableItem craftableItem in workbench.CraftableItems)
        {
            if (craftableItem == null)
                continue;

            GameObject obj = Instantiate(_CraftableItemPrefab, _CraftingListArea);

            UICraftableItemObject craftableObject = obj.GetComponent<UICraftableItemObject>();

            craftableObject.CraftableItem = craftableItem;
            craftableItem.Object = craftableObject;

            _CraftableItemObjects.Add(obj);
        }
    }

    public void ShowDescription(CraftableItem craftableItem)
    {
        if (craftableItem == null)
            return;

        if (_SelectedItem != null && _SelectedItem != craftableItem)
            _SelectedItem.Object.Deselect();

        _Description.SetActive(true);

        _SelectedItem = craftableItem;

        string resReqs = string.Empty;
        foreach  (ResourceRequirement<string, int> resReq in craftableItem.ResourceReqs)
        {
            resReqs += resReq.Amount + " " + resReq.ItemID;

            int last = (craftableItem.ResourceReqs.Length - 1);

            string id0 = resReq.ItemID;
            string id1 = craftableItem.ResourceReqs[last].ItemID;

            int am0 = resReq.Amount;
            int am1 = craftableItem.ResourceReqs[last].Amount;

            if (id0 != id1 && am0 != am1)
                resReqs += "\n";
        }

        _DescriptionText.text = resReqs;
        _DescriptionImage.sprite = craftableItem.Sprite;
    }

    public void Open()
    {
        _CanvasGroup.alpha = 1.0f;
        _CanvasGroup.blocksRaycasts = true;
    }

    public void Exit()
    {
        _CanvasGroup.alpha = 0.0f;
        _CanvasGroup.blocksRaycasts = false;

        _Description.SetActive(false);
    }

    public bool CanCraft(CraftableItem craftableItem)
    {
        // check player's inventory 

        foreach (ResourceRequirement<string, int> resReq in craftableItem.ResourceReqs)
        {
            string itemID = resReq.ItemID;
            int amount = resReq.Amount;

            // check inventory if it contains amount of itemID, if not return false
        }

        return true;
    }
}
