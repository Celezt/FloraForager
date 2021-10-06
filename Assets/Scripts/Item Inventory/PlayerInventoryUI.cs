using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInventoryUI : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public bool UIActive;

    public PlayerAction playerAction;
    public void Awake()
    {
        playerAction = new PlayerAction();
    }
    public void OnEnable()
    {
        playerAction.Enable();
        playerAction.Ground.Inventory.started += OnInventory;
    }
    public void OnDisable()
    {
        playerAction.Disable();
        playerAction.Ground.Inventory.started -= OnInventory;
    }

    public void OnInventory(InputAction.CallbackContext context) 
    {
        if (UIActive)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            UIActive = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            UIActive = true;
        }

    }
}
