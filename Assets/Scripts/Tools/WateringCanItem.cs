using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class WateringCanItem : IUse, IStar
{
    [OdinSerialize, PropertyOrder(float.MinValue)]
    public float Cooldown { get; set; } = 0.5f;
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    public int ItemStack { get; set; } = 1;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    public Stars Star { get; set; } = Stars.One;

    [Title("Tool Behaviour")]
    [SerializeField]
    private float _Radius = 2.5f;
    [SerializeField]
    private float _Arc = 0.9f;
    [SerializeField]
    private int _AdditionalUses = 3;

    private int _MaxUses;
    private int _UsesLeft;

    public void OnInitialize(ItemTypeContext context)
    {
        _MaxUses = (int)Star * 2 + _AdditionalUses;
        _UsesLeft = 0;
    }

    public void OnEquip(ItemContext context)
    {

    }

    public void OnUnequip(ItemContext context)
    {

    }

    public void OnUpdate(ItemContext context)
    {

    }

    public IEnumerable<IUsable> OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.NameToLayer("Grid"));

        if (PointInArc(hitInfo.point, context.transform.position, context.transform.eulerAngles.y, _Radius))
        {
            Cell cell = Grid.Instance.HoveredCell;
            if (cell.Type == CellType.Water)
            {
                _UsesLeft = _MaxUses;
            }
            else if (cell.HeldObject != null && _UsesLeft > 0)
            {
                if (cell.HeldObject.TryGetComponent(out FloraObject floraObject))
                {
                    if (floraObject.Flora.Water())
                    {
                        --_UsesLeft;
                    }
                }
            }
        }

        yield break;
    }

    private bool PointInArc(Vector3 point, Vector3 position, float directionAngle, float radius)
    {
        float range = Mathf.Sqrt(
            Mathf.Pow(point.x - position.x, 2) +
            Mathf.Pow(point.z - position.z, 2));
        float angle = Mathf.Atan2(
            point.z - position.z,
            point.x - position.x) * Mathf.Rad2Deg;

        float leftAngle = 90.0f - directionAngle - Mathf.Rad2Deg * _Arc;
        float rightAngle = 90.0f - directionAngle + Mathf.Rad2Deg * _Arc;

        bool inArc = false;
        if (range > radius)
        {
            inArc = false;
        }
        else if (leftAngle < rightAngle)
        {
            if (angle > leftAngle && angle < rightAngle)
            {
                inArc = true;
            }
        }
        else if (leftAngle > rightAngle)
        {
            if (angle > leftAngle)
            {
                inArc = true;
            }
            else if (angle < rightAngle)
            {
                inArc = true;
            }
        }

        return inArc;
    }
}
