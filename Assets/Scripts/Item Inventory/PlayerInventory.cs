using IngameDebugConsole;
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
    private GameObject _inventoryLayout;
    [SerializeField]
    private GameObject _hotbarLayout;
    [SerializeField]
    private string _openSound = "open_window";
    [SerializeField]
    private string _closeSound = "close_window";
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
        if (DebugManager.IsFocused)
            return;

        float value = context.ReadValue<float>();

        if (_isInventoryOpen || value > _hotbarHandler.Slots.Count)
            return;

        if (PlayerInput.GetPlayerByIndex(_playerIndex).GetComponent<UseBehaviour>().Cooldown.Item2.IsActive)
            return;

        _inventory.SetSelectedItem((int)(value - 1.0f));
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (DebugManager.IsFocused)
            return; 

        _isInventoryOpen = !_isInventoryOpen;

        if (_isInventoryOpen)
        {
            UIStateVisibility.Instance.Hide("player_hud");
            SoundPlayer.Instance.Play(_openSound);

            _inventoryLayout.SetActive(true);
            _hotbarLayout.SetActive(false);

            Time.timeScale = 0;
            PlayerInput.GetPlayerByIndex(_playerIndex).DeactivateInput();
        }
        else
        {
            UIStateVisibility.Instance.Show("player_hud");
            SoundPlayer.Instance.Play(_closeSound);

            _hotbarLayout.SetActive(true);
            _inventoryLayout.SetActive(false);

            Time.timeScale = 1;
            PlayerInput.GetPlayerByIndex(_playerIndex).ActivateInput();
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

    public void Give(string id, int amount) => _inventory.InsertUntilFull(id, amount);
    public void Remove(string id, int amount) => _inventory.Remove(id, amount);

    private void Awake()
    {
        _playerAction = new PlayerAction();

        DebugLogConsole.AddCommandInstance("player.give", "Adds item to existing player inventory", nameof(Give), this);
        DebugLogConsole.AddCommandInstance("player.remove", "Removes item from existing player inventory", nameof(Remove), this);
    }

    private void Start()
    {
        //Show(_hotbarHandler.GetComponent<CanvasGroup>());
        //Hide(_inventoryHandler.GetComponent<CanvasGroup>());
        _hotbarLayout.SetActive(true);
        _inventoryLayout.SetActive(false);

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
            {
                _hotbarFrameTransforms.Add(_hotbarHandler.Slots[i].FrameTransform);

                if (i >= 0 && i < _inventory.Items.Count)
                    HideBackground(i, _inventory.Get(i));
            }
        };

        _inventory.OnItemMoveCallback += (beforeIndex, afterIndex, beforeItem, afterItem) =>
        {
            int count = _hotbarHandler.Slots.Count;

            // from hotbar to inventory
            if (beforeIndex < count && afterIndex >= count)
            {
                if (beforeIndex == _selectedIndex)
                {
                    if (string.IsNullOrEmpty(afterItem.ID))
                        _inventory.SelectFirst();
                    else
                        _inventory.SetSelectedItem(_selectedIndex);
                }
            }

            // from inventory to hotbar
            if (beforeIndex >= count && afterIndex < count)
            {
                if (string.IsNullOrEmpty(_inventory.Get(_selectedIndex).ID) || _selectedIndex >= count)
                {
                    _inventory.SetSelectedItem(afterIndex);
                    _selectedIndex = afterIndex;
                }
            }

            // from hotbar to hotbar
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
            if (index < 0 || index >= _hotbarHandler.Slots.Count)
                return;

            if (string.IsNullOrEmpty(_inventory.Get(index).ID))
                return;

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
