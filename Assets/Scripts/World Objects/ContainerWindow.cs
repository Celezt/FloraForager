using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public class ContainerWindow : Singleton<ContainerWindow>
{
    [SerializeField]
    private InventoryHandler _playerInvHandler;
    [SerializeField]
    private GameObject _containerInv;

    private InventoryHandler _containerInvHandler;

    private void Start()
    {
        PlayerInfo playerInfo = PlayerInput.GetPlayerByIndex(0).GetComponent<PlayerInfo>();

        _playerInvHandler.Inventory = playerInfo.Inventory;
        _playerInvHandler.IsItemSelectable = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Open(List<ItemAsset> containerItems)
    {
        _containerInvHandler = _containerInv.AddComponent<InventoryHandler>();

        Inventory inventory = new Inventory();

        _containerInvHandler.Inventory = inventory;
        _containerInvHandler.IsItemSelectable = false;

        StartCoroutine(InitializeItems(inventory, containerItems));
    }

    public void Close()
    {
        Destroy(_playerInvHandler);
        Destroy(_containerInvHandler);
    }

    private IEnumerator InitializeItems(Inventory inventory, List<ItemAsset> items)
    {
        yield return null;
        inventory.Initialize(items);
    }
}
