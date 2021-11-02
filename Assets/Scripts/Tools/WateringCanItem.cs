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
    private int _MaxUses = 5;
    [Space(10)]
    [SerializeField]
    private float _Radius = 2.75f;
    [SerializeField]
    private float _Arc = 1.6f;

    private int _UsesLeft;

    public void OnInitialize(ItemTypeContext context)
    {
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

        Cell cell;
        if ((cell = Grid.Instance.HoveredCell) == null)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.NameToLayer("Grid"));

        if (MathUtility.PointInArc(hitInfo.point, context.transform.position, context.transform.localEulerAngles.y, _Arc, _Radius))
        {
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
}
