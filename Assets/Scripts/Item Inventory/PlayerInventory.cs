using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInventory : MonoBehaviour
{
    public int PlayerIndex => _playerIndex;

    [SerializeField]
    private InventoryHandler _inventoryHandler;
    [SerializeField]
    private InventoryHandler _hotbarHandler;
    [SerializeField]
    private float _frameDegreeSpeed = 4.0f;
    [SerializeField]
    private Color _selectColor = Color.white;

    private List<RectTransform> _hotbarFrameTransforms = new List<RectTransform>();

    private Inventory _inventory;
    private PlayerAction _playerAction;

    private bool _isInventoryOpen;
    private float _frameDegree;
    private int _selectedIndex;
    private int _playerIndex;

    public void OnHotbar(InputAction.CallbackContext context)
    {
        if (DebugManager.DebugMode)
            return;

        float value = context.ReadValue<float>();

        if (_isInventoryOpen)
            return;

        if (value > _hotbarHandler.Slots.Count)
            return;

        _inventory.SetSelectedItem((int)(value - 1.0f));
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (DebugManager.DebugMode)
            return; 

        _isInventoryOpen = !_isInventoryOpen;

        if (_isInventoryOpen)
        {
            UIStateVisibility.Instance.Hide("player_hud");

            Show(_inventoryHandler.GetComponent<CanvasGroup>());
            Hide(_hotbarHandler.GetComponent<CanvasGroup>());

            Time.timeScale = 0;
        }
        else
        {
            UIStateVisibility.Instance.Show("player_hud");

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
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
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

        //if (!GameManager.Instance.Stream.Get<Inventory>(_id).TryGetTarget(out _inventory))
        //    Debug.LogError("Player inventory was not found");

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);

        _playerIndex = playerInput.playerIndex;
        _inventory = playerInput.GetComponent<PlayerInfo>().Inventory;

        _inventoryHandler.Inventory = _inventory;
        _inventoryHandler.IsItemSelectable = false;
        _hotbarHandler.Inventory = _inventory;
        _hotbarHandler.IsItemSelectable = true;

        void SetColorWithoutTransparency(Color newColor)
        {
            Color color = _hotbarHandler.Slots[_selectedIndex].Background.color;
            color = new Color(newColor.r, newColor.g, newColor.b, color.a);
            _hotbarHandler.Slots[_selectedIndex].Background.color = color;
        }

        void HideBackground(int index, ItemAsset item)
        {
            if (index < _hotbarHandler.Slots.Count)
            {
                if (string.IsNullOrEmpty(item.ID) || item.Amount <= 0)  // Hide background if slot is empty.
                {
                    Color color = _hotbarHandler.Slots[index].Background.color;
                    color.a = 0;
                    _hotbarHandler.Slots[index].Background.color = color;
                }
                else
                {
                    Color color = _hotbarHandler.Slots[index].Background.color;
                    color.a = 1;
                    _hotbarHandler.Slots[index].Background.color = color;
                }
            }
        }

        _hotbarHandler.OnInventoryInitalizedCallback += () =>
        {
            _inventory.SetSelectedItem(0);

            for (int i = 0; i < _hotbarHandler.Slots.Count; i++)
                _hotbarFrameTransforms.Add(_hotbarHandler.Slots[i].FrameTransform);
        };

        _inventory.OnItemMoveCallback += (beforeIndex, afterIndex, beforeItem, afterItem) =>
        {
            int count = _hotbarHandler.Slots.Count;
            if (beforeIndex < count && afterIndex >= count)
            {
                _inventory.SelectFirst();
            }

            if (beforeIndex < count && afterIndex < count)
            {
                if (beforeIndex == _selectedIndex) // Change selected's color.
                {
                    SetColorWithoutTransparency(Color.white);
                    _selectedIndex = afterIndex;
                    SetColorWithoutTransparency(_selectColor);
                }
            }
        };

        _inventory.OnSelectItemCallback += (index, item) =>
        {

            if (_selectedIndex != index)    // Change selected's color.
                SetColorWithoutTransparency(Color.white);

            _selectedIndex = index;
            SetColorWithoutTransparency(_selectColor);
        };

        _inventory.OnRemoveItemCallback += (index, item) =>
        {
            HideBackground(index, item);
        };

        _inventory.OnItemChangeCallback += (index, item) =>
        {
            HideBackground(index, item);
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
