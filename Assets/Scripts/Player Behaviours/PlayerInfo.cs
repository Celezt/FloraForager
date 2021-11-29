using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField]
    private float _FadeOutTime = 2.0f;
    [SerializeField]
    private List<ItemAsset> _InitialItems;

    private PlayerData _Data;
    private Inventory _Inventory;
    private PlayerStamina _Stamina;

    private bool _DataExists;

    public PlayerData Data => _Data;
    public Inventory Inventory => _Inventory;
    public PlayerStamina Stamina => _Stamina;

    private void Awake()
    {
        int playerIndex = GetComponent<PlayerInput>().playerIndex;

        _DataExists = PlayerDataMaster.Instance.Exists(playerIndex);
        _Data = PlayerDataMaster.Instance.Get(playerIndex);

        _Inventory = new Inventory();
        _Stamina = GetComponent<PlayerStamina>();

        if (_DataExists)
        {
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();

            transform.position = _Data.SaveData.Position;
            playerMovement.SetDirection((_Data.SaveData.Rotation * Vector3.forward).xz());
        }
    }

    private void Start()
    {
        List<ItemAsset> initialItems = _DataExists ?
            _Data.SaveData.Items : _InitialItems;

        _Inventory.Initialize(initialItems);

        FadeScreen.Instance.StartFadeOut(_FadeOutTime);
    }

    private void OnDestroy()
    {
        _Data.SaveData.Items = _Inventory.Items.ToList();
        _Data.SaveData.Stamina = _Stamina.Stamina;
    }
}
 