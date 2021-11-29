using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class PlaceableItem : IUse
{
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    float IUse.Cooldown { get; set; } = 0;

    [Title("Place Behaviour")]
    [SerializeField]
    private string _placeSound = "place_object";
    [SerializeField]
    private float _placeRange = 3.0f;
    [SerializeField]
    private bool _followNormal;
    [SerializeField]
    private LayerMask _placeMask = LayerMask.GetMask("Ground");
    [SerializeField, ListDrawerSettings(Expanded = true)]
    private CellType[] _allowedTypes = new CellType[1];
    [SerializeField, AssetsOnly]
    private Material _material;
    [SerializeField, AssetsOnly]
    private GameObject _placeableObject;

    private Quaternion _rotation = Quaternion.identity;

    private Cell _cell;

    private GameObject _objectAux;
    private MeshFilter _objectAuxFilter;
    private MeshRenderer _objectAuxRenderer;

    private bool _canPlace;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
        _objectAux = new GameObject("placeable auxiliary", typeof(MeshRenderer), typeof(MeshFilter));

        _objectAuxFilter = _objectAux.GetComponent<MeshFilter>();
        _objectAuxRenderer = _objectAux.GetComponent<MeshRenderer>();

        _objectAuxFilter.mesh = _placeableObject.GetComponent<MeshFilter>().sharedMesh;
        _objectAuxRenderer.material = _material;
    }

    void IItem.OnUnequip(ItemContext context)
    {
        Object.Destroy(_objectAux);
    }

    void IItem.OnUpdate(ItemContext context)
    {
        _cell = GameGrid.Instance.HoveredCell;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, 50f, _placeMask);

        float distance = Mathf.Sqrt(
            Mathf.Pow(hitInfo.point.x - context.transform.position.x, 2) +
            Mathf.Pow(hitInfo.point.z - context.transform.position.z, 2));

        if (distance <= _placeRange && _cell != null)
        {
            _canPlace = true;
            _objectAuxRenderer.enabled = true;

            _objectAux.transform.position = _cell.Middle;
        }
        else
        {
            _canPlace = false;
            _objectAuxRenderer.enabled = false;
        }
    }

    IEnumerator IUse.OnUse(UseContext context)
    {
        if (!context.started || !_canPlace || _cell == null)
            yield break;

        if (!_canPlace)
            yield break;

        foreach (CellType type in _allowedTypes)
        {
            if (type == _cell.Type)
            {
                context.Place(_placeableObject, _cell, _rotation, _followNormal);
                context.Consume();

                SoundPlayer.Instance.Play(_placeSound);

                yield break;
            }
        }

        yield break;
    }
}
