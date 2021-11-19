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
    private int _MaxUses = 5;
    [SerializeField]
    private string _wateringSound;
    [SerializeField]
    private string _fillSound;
    [SerializeField]
    private AnimationClip _wateringClip;
    [SerializeField]
    private AnimationClip _fillClip;
    [SerializeField, AssetsOnly]
    private GameObject _model;
    [SerializeField]
    private float _stunWateringDuration = 1.2f;
    [SerializeField]
    private float _stunFillDuration = 1.2f;
    [SerializeField]
    private float _onWateringUse = 0.5f;
    [SerializeField]
    private float _onFillUse = 0.8f;
    [Space(10)]
    [SerializeField]
    private LayerMask _HitMask = LayerMask.NameToLayer("Grid");
    [SerializeField]
    private float _Radius = 2.75f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _Arc = 270.0f;

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

    public IEnumerator OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Cell cell;
        if ((cell = Grid.Instance.HoveredCell) == null)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _HitMask);

        if (MathUtility.PointInArc(hitInfo.point, context.transform.position, context.transform.localEulerAngles.y, _Arc, _Radius))
        {
            if (cell.Type == CellType.Water)
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

                SoundPlayer.Instance.Play(_fillSound);

                yield return new WaitForSeconds(_onFillUse);

                _UsesLeft = _MaxUses;
            }
            else if (cell.HeldObject != null && _UsesLeft > 0)
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

                        SoundPlayer.Instance.Play(_wateringSound);

                        yield return new WaitForSeconds(_onWateringUse);

                        --_UsesLeft;
                    }
                }
            }
        }

        yield break;
    }
}
