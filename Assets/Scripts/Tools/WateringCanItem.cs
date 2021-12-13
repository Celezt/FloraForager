using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class WateringCanItem : IUse, IStar
{
    [OdinSerialize, PropertyOrder(float.MinValue)]
    public float Cooldown { get; set; } = 1.4f;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    public Stars Star { get; set; } = Stars.One;

    [Title("Tool Behaviour")]
    [SerializeField]
    private int _maxUses = 5;
    [SerializeField]
    private float _fillStaminaChange = -0.05f;
    [SerializeField]
    private float _waterStaminaChange = -0.04f;
    [Space(10)]
    [SerializeField]
    private string _wateringSound = "water_plant";
    [SerializeField]
    private string _fillSound = "fill_watering-can";
    [SerializeField]
    private AnimationClip _wateringClip;
    [SerializeField]
    private AnimationClip _fillClip;
    [SerializeField, AssetsOnly]
    private GameObject _model;
    [SerializeField]
    private float _stunWateringDuration = 1.3f;
    [SerializeField]
    private float _stunFillDuration = 1.6f;
    [SerializeField]
    private float _onWateringUse = 0.4f;
    [SerializeField]
    private float _onFillUse = 0.8f;
    [Space(10)]
    [SerializeField]
    private float _radius = 2.0f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _arc = 140.0f;
    [SerializeField, ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true, ShowItemCount = false, DraggableItems = false)]
    private CellType[] _allowedUse = new CellType[] { CellType.Dirt, CellType.Water };

    private PlayerStamina _playerStamina;
    private int _usesLeft;

    public int UsesLeft => _usesLeft;
    public int MaxUses => _maxUses;

    public void OnInitialize(ItemTypeContext context)
    {
        _usesLeft = 0;
    }

    public void OnEquip(ItemContext context)
    {
        _playerStamina = context.transform.GetComponent<PlayerStamina>();
        WateringCanUI.Instance.Show(this);
    }

    public void OnUnequip(ItemContext context)
    {
        WateringCanUI.Instance.Hide();
    }

    public void OnUpdate(ItemContext context)
    {

    }

    public IEnumerator OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Cell cell;
        if ((cell = GameGrid.Instance.HoveredCell) == null || !_allowedUse.Contains(cell.Type))
        {
            SoundPlayer.Instance.Play("use_error");
            yield break;
        }

        if (MathUtility.PointInArc(GameGrid.Instance.MouseHit, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
        {
            if (cell.Type == CellType.Water) // fill watering can
            {
                context.behaviour.ApplyCooldown();

                GameObject model = null;

                context.transform.GetComponentInChildren<PlayerMovement>().ActivaInput.Add(_stunFillDuration);
                context.transform.GetComponentInChildren<HumanoidAnimationBehaviour>().CustomMotionRaise(_fillClip,
                    enterCallback: info =>
                    {
                        if (_model == null)
                            return;

                        model = Object.Instantiate(_model, info.animationBehaviour.HoldTransform);
                    },
                    exitCallback: info =>
                    {
                        if (_model == null)
                            return;

                        Object.Destroy(model);
                    }
                );

                yield return new WaitForSeconds(_onFillUse);

                SoundPlayer.Instance.Play(_fillSound);

                _usesLeft = _maxUses;
                _playerStamina.Stamina += _fillStaminaChange;
            }
            else if (_usesLeft > 0 && cell.Type == CellType.Dirt && cell.HeldObject != null)
            {
                if (cell.HeldObject.TryGetComponent(out FloraObject floraObject))
                {
                    if (!floraObject.Flora.Watered)
                    {
                        context.behaviour.ApplyCooldown();

                        GameObject model = null;

                        context.transform.GetComponentInChildren<PlayerMovement>().ActivaInput.Add(_stunWateringDuration);
                        context.transform.GetComponentInChildren<HumanoidAnimationBehaviour>().CustomMotionRaise(_wateringClip,
                            enterCallback: info =>
                            {
                                if (_model == null)
                                    return;

                                model = Object.Instantiate(_model, info.animationBehaviour.HoldTransform);
                            },
                            exitCallback: info =>
                            {
                                if (_model == null)
                                    return;

                                Object.Destroy(model);
                            }
                        );

                        yield return new WaitForSeconds(_onWateringUse);

                        if (SoundPlayer.Instance.TryGetSound(_wateringSound, out SoundPlayer.Sound sound))
                            SoundPlayer.Instance.Play(_wateringSound, -sound.Volume / _maxUses * (_maxUses - _usesLeft));

                        --_usesLeft;
                        _playerStamina.Stamina += _waterStaminaChange;

                        floraObject.Flora.Water();
                    }
                }
            }
            else
                SoundPlayer.Instance.Play("use_error");
        }

        yield break;
    }
}
