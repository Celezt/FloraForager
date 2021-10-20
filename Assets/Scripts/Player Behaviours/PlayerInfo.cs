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

    private void Start()
    {
        GameManager.StreamScriptableObject stream = GameManager.Instance.Stream;

        stream.Get<Inventory>("player_inventory_0").TryGetTarget(out _inventory);
        stream.Get<PlayerData>("player_data_0").TryGetTarget(out _playerData);
    }
}
 