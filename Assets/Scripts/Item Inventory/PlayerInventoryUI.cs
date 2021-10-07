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
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
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
            UIActive = false;
        }
        else
        {
            UIActive = true;
        }
        UIActivation();
    }
    public void UIActivation() 
    {
        if (UIActive)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;            
        }
    }
}
