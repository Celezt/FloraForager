using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Workbench : MonoBehaviour, IInteractable
{
    [SerializeField] private float _Radius = 3.5f;
    [SerializeField] private CraftableItemData[] _CraftableItemsData;

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
    }

    private void Update()
    {
        if (UICraftingMenu.Instance.Workbench == null || _Player == null)
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

        UICraftingMenu.Instance.ShowCraftableItems(this);
        UICraftingMenu.Instance.Open();
    }
}
