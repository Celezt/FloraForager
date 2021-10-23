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
        _inventory = ScriptableObject.CreateInstance<Inventory>();
    }

    private void Start()
    {
        //stream.Get<Inventory>("player_inventory_0").TryGetTarget(out _inventory);
        //stream.Get<PlayerData>("player_data_0").TryGetTarget(out _playerData);
    }
}
 