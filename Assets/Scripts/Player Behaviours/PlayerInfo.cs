using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public Inventory Inventory => _inventory;
    public PlayerData Data => _playerData;

    [SerializeField] private Inventory _inventory;
    [SerializeField] private PlayerData _playerData;
}
