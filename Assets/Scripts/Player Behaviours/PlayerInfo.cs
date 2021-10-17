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

        GameManager.StreamScriptableObject stream = GameManager.Instance.Stream;

        _inventory = stream.LoadPersistent<Inventory>($"PlayerInventory{playerIndex}");
        _playerData = stream.LoadPersistent<PlayerData>($"PlayerData{playerIndex}");
    }
}
 