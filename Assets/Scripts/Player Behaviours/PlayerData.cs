using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerData : IStreamable<PlayerData.Data>
{
    private Data _Data;

    public Data SaveData => _Data;

    public class Data
    {
        public int PlayerIndex;

        public Vector3 Position;
        public Quaternion Rotation;

        public float Stamina;
        public List<ItemAsset> Items;
        public int SceneIndex;
    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }
    public void OnBeforeSaving()
    {
        PlayerInput player = PlayerInput.GetPlayerByIndex(_Data.PlayerIndex);

        if (player == null)
            return;

        PlayerInfo playerInfo = player.GetComponent<PlayerInfo>();

        PlayerStamina playerStamina = playerInfo.Stamina;
        Inventory playerInventory = playerInfo.Inventory;

        _Data.Position = player.transform.position;
        _Data.Rotation = player.transform.rotation;

        _Data.Stamina = playerStamina.Stamina;
        _Data.Items = playerInventory.Items.ToList();
        _Data.SceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public PlayerData(int playerIndex)
    {
        _Data = new Data();

        _Data.PlayerIndex = playerIndex;
        _Data.Items = new List<ItemAsset>();
    }
}
