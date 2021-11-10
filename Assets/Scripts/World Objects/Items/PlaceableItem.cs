using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class PlaceableItem : IUse
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 64;

    [SerializeField]
    private float _placeRange = 3.0f;
    [SerializeField]
    private bool _followNormal;
    [SerializeField]
    private LayerMask _placeMask = LayerMask.GetMask("Ground");
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private CellType[] _AllowedTypes = new CellType[1];
    [SerializeField, AssetList(Path = "Prefabs/Placeables"), AssetsOnly]
    private GameObject _placeableObject;

    float IUse.Cooldown { get; set; } = 0;

    private Quaternion _Rotation = Quaternion.identity;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
        
    }

    void IItem.OnUnequip(ItemContext context)
    {

    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerator IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _placeMask);

        float distance = Mathf.Sqrt(
            Mathf.Pow(hitInfo.point.x - context.transform.position.x, 2) + 
            Mathf.Pow(hitInfo.point.z - context.transform.position.z, 2));

        if (distance >= _placeRange)
            yield break;

        Cell cell = Grid.Instance.HoveredCell;

        if (cell == null)
            yield break;

        foreach (CellType type in _AllowedTypes)
        {
            if (type == cell.Type)
            {
                context.Place(_placeableObject, cell, _Rotation, _followNormal);
                context.Consume();

                yield break;
            }
        }

        yield break;
    }
}
