using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.InputSystem;
using TMPro;
using MyBox;

public class UICraftingMenu : Singleton<UICraftingMenu>
{
    [SerializeField] private GameObject _CraftableItemPrefab;
    [SerializeField] private Transform _CraftingListArea;

    [SerializeField] private GameObject _Description;
    [SerializeField] private TMP_Text _ItemNameText;
    [SerializeField] private Image _ItemStarsImage;
    [SerializeField] private Image _ItemImage;
    [SerializeField] private TMP_Text _RequirementsText;

    [SerializeField] private SpriteAtlas _StarsAtlas;

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

        Inventory inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;

        foreach (ItemAsset requirement in _SelectedItem.Requirements)
        {
            inventory.Remove(requirement.ID, requirement.Amount);
        }

        inventory.Insert(new ItemAsset
        {
            ID = _SelectedItem.ItemID,
            Amount = 1
        });
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

        string requirements = string.Empty;
        for (int i = 0; i < craftableItem.Requirements.Length; ++i)
        {
            ItemAsset requirement = craftableItem.Requirements[i];

            requirements += requirement.Amount + " " + ItemTypeSettings.Instance.ItemNameChunk[requirement.ID];

            if (i != (craftableItem.Requirements.Length - 1))
                requirements += "\n";
        }

        Stars? star = (ItemTypeSettings.Instance.ItemTypeChunk[craftableItem.ItemID].Behaviour as IStar)?.Star;
        
        int index = star.HasValue ? (int)star : 0;
        Sprite starSprite = _StarsAtlas.GetSprite($"stars_{index}");

        _ItemNameText.text = ItemTypeSettings.Instance.ItemNameChunk[craftableItem.ItemID];
        _ItemStarsImage.sprite = starSprite;
        _ItemImage.sprite = ItemTypeSettings.Instance.ItemIconChunk[craftableItem.ItemID];
        _RequirementsText.text = requirements;


        LayoutRebuilder.ForceRebuildLayoutImmediate(_Description.GetComponent<RectTransform>());
    }

    public void Open()
    {
        _CanvasGroup.alpha = 1.0f;
        _CanvasGroup.blocksRaycasts = true;
    }

    public void Exit()
    {
        _Workbench = null;

        _CanvasGroup.alpha = 0.0f;
        _CanvasGroup.blocksRaycasts = false;

        _Description.SetActive(false);
    }

    public bool CanCraft(CraftableItem craftableItem)
    {
        Inventory inventory = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>().Inventory;

        foreach (ItemAsset requirement in craftableItem.Requirements)
        {
            if (!inventory.FindEnough(requirement.ID, requirement.Amount))
                return false;
        }

        return true;
    }
}
