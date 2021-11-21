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
    private LayerMask _hitMask = LayerMask.NameToLayer("Grid");
    [SerializeField]
    private float _radius = 2.0f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _arc = 140.0f;

    private PlayerStamina _playerStamina;
    private int _usesLeft;

    public void OnInitialize(ItemTypeContext context)
    {
        _usesLeft = 0;
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
        if ((cell = Grid.Instance.HoveredCell) == null)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _hitMask);

        if (MathUtility.PointInArc(hitInfo.point, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
        {
            if (cell.Type == CellType.Water) // fill watering can
            {
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
            else if (cell.HeldObject != null && _usesLeft > 0)
            {
                if (cell.HeldObject.TryGetComponent(out FloraObject floraObject))
                {
                    if (floraObject.Flora.Water())
                    {
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

                        SoundPlayer.Instance.Play(_wateringSound);

                        --_usesLeft;
                        _playerStamina.Stamina += _waterStaminaChange;
                    }
                }
            }
        }

        yield break;
    }
}
