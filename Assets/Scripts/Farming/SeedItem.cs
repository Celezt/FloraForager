using System.Linq;
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
    private float _staminaChange = -0.1f;
    [Space(10)]
    [SerializeField]
    private AnimationClip _sowClip;
    [SerializeField]
    private float _stunDuration = 2.333f;
    [SerializeField]
    private float _onUse = 1.1f;
    [Space(10)]
    [SerializeField]
    private float _radius = 1.1f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _arc = 160.0f;
    [SerializeField, ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true, ShowItemCount = false, DraggableItems = false)]
    private CellType[] _allowedUse = new CellType[] { CellType.Dirt };

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

        Cell cell;
        if ((cell = GameGrid.Instance.HoveredCell) == null || GameGrid.Instance.HoveredCell.Occupied ||
            !_allowedUse.Contains(GameGrid.Instance.HoveredCell.Type))
        {
            SoundPlayer.Instance.Play("use_error");
            yield break;
        }

        if (MathUtility.PointInArc(GameGrid.Instance.MouseHit, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
        {
            context.behaviour.ApplyCooldown();

            context.transform.GetComponentInChildren<PlayerMovement>().ActivaInput.Add(_stunDuration);
            context.transform.GetComponentInChildren<AnimationBehaviour>().CustomMotionRaise(_sowClip);

            yield return new WaitForSeconds(_onUse);

            FloraMaster.Instance.Add(_flora, cell);

            SoundPlayer.Instance.Play("place_seed");

            context.Consume();
            _playerStamina.Stamina += _staminaChange;
        }

        yield break;
    }
}
