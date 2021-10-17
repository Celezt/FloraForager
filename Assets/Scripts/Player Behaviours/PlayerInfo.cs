using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInfo : MonoBehaviour
{
    public Inventory Inventory => _inventory;
    public PlayerData Data => _playerData;

    private Inventory _inventory;
    private PlayerData _playerData;

    private void Awake()
    {
        int playerIndex = GetComponent<PlayerInput>().playerIndex;

        _inventory = GameManager.Instance.Stream.LoadPersistent<Inventory>($"PlayerInventory{playerIndex}");
        _playerData = GameManager.Instance.Stream.LoadPersistent<PlayerData>($"PlayerData{playerIndex}");
    }
}
 