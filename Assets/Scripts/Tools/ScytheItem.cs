using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using Celezt.Mathematics;
using MyBox;

public class ScytheItem : IUse, IDestructor, IStar, IValue
{
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    int IValue.BaseValue { get; set; }
    [OdinSerialize, PropertyOrder(float.MinValue + 3)]
    float IUse.Cooldown { get; set; } = 0.5f;
    [OdinSerialize, PropertyOrder(float.MinValue + 4)]
    float IDestructor.Damage { get; set; } = 2.0f;

    [Title("Tool Behaviour")]
    [SerializeField]
    private float _staminaChange = -0.075f;
    [SerializeField]
    private string _swingSound = "swing_tool";
    [SerializeField]
    private string _poorSound = "hit_poor";
    [SerializeField]
    private AnimationClip _clip;
    [SerializeField, AssetsOnly]
    private GameObject _model;
    [SerializeField]
    private float _stunDuration = 0.5f;
    [SerializeField]
    private float _onSwing = 0.1f;
    [SerializeField, Sirenix.OdinInspector.MinValue("_onSwing")]
    private float _onUse = 0.2f;
    [SerializeField]
    private float _radius = 3f;
    [SerializeField]
    private float _arc = 0.4f;

    private PlayerStamina _playerStamina;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
        _playerStamina = context.transform.GetComponent<PlayerStamina>();

#if UNITY_EDITOR
        context.behaviour.OnDrawGizmosAction = () =>
        {
            Gizmos.matrix = context.transform.localToWorldMatrix;
            GizmosC.DrawWireArc(Vector3.zero, Vector3.forward, _radius, cmath.Map(_arc, new MinMaxFloat(1, -1), new MinMaxFloat(0, 360)));
        };
#endif
    }

    void IItem.OnUnequip(ItemContext context)
    {

    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerator IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        context.behaviour.ApplyCooldown();

        GameObject model = null;

        context.transform.GetComponentInChildren<PlayerMovement>().ActivaInput.Add(_stunDuration);
        context.transform.GetComponentInChildren<AnimationBehaviour>().Play(_clip,
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

        yield return new WaitForSeconds(_onSwing);

        SoundPlayer.Instance.Play(_swingSound);

        yield return new WaitForSeconds(_onUse - _onSwing);

        _playerStamina.Stamina += _staminaChange;

        Collider[] colliders = PhysicsC.OverlapArc(context.transform.position, context.transform.forward, Vector3.up, _radius, _arc, LayerMask.NameToLayer("default"));

        for (int i = 0; i < colliders.Length; i++)
        {
            IUsable usable;
            if ((usable = colliders[i].GetComponentInParent<IUsable>()) != null)
            {
                if (!context.CallUsable(usable))
                { 
                    MessageLog.Instance.Send("Unsuitable Tool", Color.red, 14f, 2f);
                    SoundPlayer.Instance.Play(_poorSound);
                }
            }
        }
    }
}
