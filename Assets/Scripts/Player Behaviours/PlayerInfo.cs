using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public InventoryObject Inventory => _inventory;
    public PlayerData Data => _playerData;

    [SerializeField] private InventoryObject _inventory;
    [SerializeField] private PlayerData _playerData;
}
