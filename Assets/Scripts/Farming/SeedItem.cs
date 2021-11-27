using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class SeedItem : IUse, IStar, IValue
{
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    int IValue.BaseValue { get; set; }
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    float IUse.Cooldown { get; set; } = 0.05f;

    [Title("Seed Behaviour")]
    [SerializeField, AssetSelector(Paths = "Assets/Data/Flora"), AssetsOnly]
    private FloraInfo _flora;
    [SerializeField]
    private float _staminaChange = -0.025f;
    [Space(10)]
    [SerializeField]
    private LayerMask _PlaceMask = LayerMask.NameToLayer("Grid");
    [SerializeField]
    private float _radius = 2.75f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _arc = 360.0f;

    private PlayerStamina _playerStamina;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    public void OnEquip(ItemContext context)
    {
        _playerStamina = context.transform.GetComponent<PlayerStamina>();
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

        if (Grid.Instance.HoveredCell == null)
            yield break;

        if (MathUtility.PointInArc(Grid.Instance.MouseHit, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
        {
            if (FloraMaster.Instance.Add(_flora))
            {
                SoundPlayer.Instance.Play("place_seed");

                context.Consume();
                _playerStamina.Stamina += _staminaChange;
            }
        }

        yield break;
    }
}
