using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class Workbench : MonoBehaviour, IInteractable
{
    [SerializeField] 
    private float _Radius = 3.5f;
    [SerializeField, AssetList(Path = "Data/Craftable Items", AutoPopulate = true)] 
    private CraftableItemInfo[] _CraftableItemsData;

    private GameObject _Player;

    private CraftableItem[] _CraftableItems;

    public CraftableItem[] CraftableItems => _CraftableItems;
    public int Priority => 3;

    private void Awake()
    {
        _CraftableItems = new CraftableItem[_CraftableItemsData.Length];
        for (int i = 0; i < _CraftableItems.Length; ++i)
        {
            _CraftableItems[i] = new CraftableItem(_CraftableItemsData[i]);
        }

        _CraftableItems = _CraftableItems.OrderBy(i => string.Join("", ItemTypeSettings.Instance.ItemLabelChunk[i.Item.ID])).ToArray();
    }

    private void Update()
    {
        if (_Player == null || UICraftingMenu.Instance.Workbench == null)
            return;

        float distance = Vector3.Distance(transform.position, _Player.transform.position);
        if (distance > _Radius && UICraftingMenu.Instance.Workbench == this)
        {
            UICraftingMenu.Instance.Exit();
        }
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        _Player = PlayerInput.GetPlayerByIndex(context.playerIndex).gameObject;

        if (!UICraftingMenu.Instance.Opened)
        {
            UICraftingMenu.Instance.ShowCraftableItems(this);
            UICraftingMenu.Instance.Open();
        }
        else
        {
            UICraftingMenu.Instance.Exit();
        }
    }
}
