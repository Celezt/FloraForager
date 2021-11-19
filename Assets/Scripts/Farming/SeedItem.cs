using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class SeedItem : IUse
{
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    float IUse.Cooldown { get; set; } = 0.05f;

    [Title("Seed Behaviour")]
    [SerializeField, AssetSelector(Paths = "Assets/Data/Flora"), AssetsOnly]
    private FloraInfo _Flora;
    [Space(10)]
    [SerializeField]
    private LayerMask _PlaceMask = LayerMask.NameToLayer("Grid");
    [SerializeField]
    private float _Radius = 2.75f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _Arc = 360.0f;

    void IItem.OnInitialize(ItemTypeContext context)
    {

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

    public IEnumerator OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _PlaceMask);

        if (MathUtility.PointInArc(hitInfo.point, context.transform.position, context.transform.localEulerAngles.y, _Arc, _Radius))
        {
            if (FloraMaster.Instance.Add(_Flora))
            {
                SoundPlayer.Instance.Play("place_seed");
                context.Consume();
            }
        }

        yield break;
    }
}
