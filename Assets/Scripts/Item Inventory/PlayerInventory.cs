using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInventory : MonoBehaviour
{
    public PlayerAction PlayerAction => _playerAction;

    [SerializeField]
    private CanvasGroup _canvasGroup;
    [SerializeField]
    private string _id;

    private bool _isActive;

    private PlayerAction _playerAction;
    private Inventory _inventory;

    public void Awake()
    {
        Hide();
        _isActive = true;

        _playerAction = new PlayerAction();
    }

    private void Start()
    {
        GameManager.Instance.Stream.Get<Inventory>(_id).TryGetTarget(out _inventory);

        InventoryHandler[] handlers = GetComponentsInChildren<InventoryHandler>();
        foreach (InventoryHandler handler in handlers)
            handler.Inventory = _inventory;
    }

    public void OnEnable()
    {
        _playerAction.Enable();
        _playerAction.Ground.Inventory.started += OnInventory;
    }

    public void OnDisable()
    {
        _playerAction.Disable();
        _playerAction.Ground.Inventory.started -= OnInventory;
    }

    public void OnInventory(InputAction.CallbackContext context) 
    {
        if (_isActive)
            Show();
        else
            Hide();

            _isActive = !_isActive;
    }

    public void Hide() 
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _isActive = false;
    }

    public void Show() 
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _isActive = true;
    }
}
