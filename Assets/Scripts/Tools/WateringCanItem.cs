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

    private WateringCanUI _waterCanUI;

    private int _usesLeft;

    public int UsesLeft => _usesLeft;
    public int MaxUses => _maxUses;

    public void OnInitialize(ItemTypeContext context)
    {
        _usesLeft = 0;
        _waterCanUI = WateringCanUI.Instance;
    }

    public void OnEquip(ItemContext context)
    {
        _waterCanUI.Show(this);
    }

    public void OnUnequip(ItemContext context)
    {
        _waterCanUI.Hide();
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

        if (!MathUtility.PointInArc(GameGrid.Instance.MouseHit, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
        {
            MessageLog.Instance.Send("Pressed Outside Usable Range", Color.red, 14f, 2f);
            SoundPlayer.Instance.Play("use_error");
            yield break;
        }

        if (cell.Type == CellType.Water) // fill watering can
        {
            context.behaviour.ApplyCooldown();

            GameObject model = null;

            context.playerInfo.PlayerMovement.ActivaInput.Add(_stunFillDuration);
            context.playerInfo.AnimationBehaviour.Play(_fillClip,
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
            context.playerInfo.PlayerStamina.Stamina += _fillStaminaChange;
        }
        else if (cell.Type == CellType.Dirt && cell.HeldObject != null && cell.HeldObject.TryGetComponent(out FloraObject floraObject))
        {
            if (!floraObject.Flora.Watered)
            {
                if (_usesLeft > 0)
                {
                    context.behaviour.ApplyCooldown();

                    GameObject model = null;

                    context.playerInfo.PlayerMovement.ActivaInput.Add(_stunWateringDuration);
                    context.playerInfo.AnimationBehaviour.Play(_wateringClip,
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

                    model.GetComponentInChildren<ParticleSystem>().Play();

                    if (SoundPlayer.Instance.TryGetSound(_wateringSound, out SoundPlayer.Sound sound))
                        SoundPlayer.Instance.Play(_wateringSound, -sound.Volume / _maxUses * (_maxUses - _usesLeft));

                    --_usesLeft;
                    context.playerInfo.PlayerStamina.Stamina += _waterStaminaChange;

                    floraObject.Flora.Water();

                    yield return new WaitForSeconds(0.8f);

                    model.GetComponentInChildren<ParticleSystem>().Stop();
                }
                else
                {
                    MessageLog.Instance.Send("Watering Can is Empty", Color.red);
                    SoundPlayer.Instance.Play("use_error");
                }
            }
            else
            {
                MessageLog.Instance.Send("Plant Already Watered", Color.red);
                SoundPlayer.Instance.Play("use_error");
            }
        }
        else
        {
            MessageLog.Instance.Send("Can't Use There", Color.red);
            SoundPlayer.Instance.Play("use_error");
        }

        yield break;
    }
}
