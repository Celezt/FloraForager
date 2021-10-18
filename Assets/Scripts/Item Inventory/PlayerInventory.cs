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
    private InventoryHandler _inventoryHandler;
    [SerializeField]
    private InventoryHandler _hotbarHandler;
    [SerializeField]
    private string _id;
    [SerializeField]
    private float _frameDegreeSpeed = 4-0f;

    private List<RectTransform> _hotbarFrameTransforms = new List<RectTransform>();

    private PlayerAction _playerAction;
    private Inventory _inventory;

    private bool _isInventoryOpen;
    private float _frameDegree;

    public void OnHotbar(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        if (_isInventoryOpen)
            return;

        if (value > _hotbarHandler.Slots.Count)
            return;

        _inventory.SetSelectedItem((int)(value - 1.0f));
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        _isInventoryOpen = !_isInventoryOpen;

        if (_isInventoryOpen)
        {
            Show(_inventoryHandler.GetComponent<CanvasGroup>());
            Hide(_hotbarHandler.GetComponent<CanvasGroup>());
            Time.timeScale = 0;
        }
        else
        {
            Show(_hotbarHandler.GetComponent<CanvasGroup>());
            Hide(_inventoryHandler.GetComponent<CanvasGroup>());
            Time.timeScale = 1;
        }
    }

    public void Hide(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void Awake()
    {
        _playerAction = new PlayerAction();
    }

    private void Start()
    {
        Show(_hotbarHandler.GetComponent<CanvasGroup>());
        Hide(_inventoryHandler.GetComponent<CanvasGroup>());

        if (!GameManager.Instance.Stream.Get<Inventory>(_id).TryGetTarget(out _inventory))
            Debug.LogError("Player inventory was not found");

        _inventoryHandler.Inventory = _inventory;
        _inventoryHandler.IsItemSelectable = false;
        _hotbarHandler.Inventory = _inventory;
        _hotbarHandler.IsItemSelectable = true;

        _hotbarHandler.OnInventoryInitalizedCallback += () =>
        {
            _inventory.SetSelectedItem(0);

            for (int i = 0; i < _hotbarHandler.Slots.Count; i++)
                _hotbarFrameTransforms.Add(_hotbarHandler.Slots[i].FrameTransform);
        };

        _hotbarHandler.Inventory.OnItemMoveCallback += (beforeIndex, afterIndex, beforeItem, afterItem) =>
        {
            if (beforeIndex < _hotbarHandler.Slots.Count && afterIndex >= _hotbarHandler.Slots.Count)
                _inventory.SelectFirst();
        };
    }

    private void Update()
    {
        _frameDegree += Time.deltaTime * _frameDegreeSpeed;
        _frameDegree %= 360;
        for (int i = 0; i < _hotbarFrameTransforms.Count; i++)
            _hotbarFrameTransforms[i].rotation = (i % 2 == 0) ? Quaternion.Euler(0, 0, _frameDegree) : Quaternion.Euler(0, 0, -_frameDegree + 45);
    }

    private void OnEnable()
    {
        _playerAction.Enable();
        _playerAction.Ground.Inventory.started += OnInventory;
        _playerAction.Ground.Hotbar.started += OnHotbar;
    }

    private void OnDisable()
    {
        _playerAction.Disable();
        _playerAction.Ground.Inventory.started -= OnInventory;
        _playerAction.Ground.Hotbar.started -= OnHotbar;
    }
}
