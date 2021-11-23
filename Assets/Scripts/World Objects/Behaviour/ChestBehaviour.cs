using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class ChestBehaviour : MonoBehaviour, IPlaceable, IInteractable
{
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private List<ItemAsset> _InitialItems;

    private ChestData _Data;
    private Inventory _Inventory;

    private string _Address = string.Empty;

    public int Priority => 1;

    private void Start()
    {
        Cell cell = Grid.Instance.GetCellWorld(transform.position);

        if (cell == null)
        {
            Debug.LogError("can only be placed on a cell");
            return;
        }

        cell.Occupy(gameObject);

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        string id = $"{cell.Local} {sceneIndex}";

        _Data = ObjectMaster.Instance.Get<ChestData>(id, gameObject, _Address, sceneIndex, cell, _InitialItems);
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        ContainerWindow.Instance.Open(_Data.ObjectData.Items);
    }

    public void OnPlace(PlacedContext context)
    {
        _Address = context.address;
    }
}
