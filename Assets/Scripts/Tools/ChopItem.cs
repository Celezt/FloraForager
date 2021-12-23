using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Linq;

public class ChopItem : IUse, IDestructor, IStar, IValue
{
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    int IValue.BaseValue { get; set; }
    [OdinSerialize, PropertyOrder(float.MinValue + 3)]
    float IUse.Cooldown { get; set; } = 1.2f;
    [OdinSerialize, PropertyOrder(float.MinValue + 4)]
    float IDestructor.Damage { get; set; } = 2.0f;

    [Title("Tool Behaviour")]
    [SerializeField]
    private float _staminaChange = -0.1f;
    [SerializeField]
    private string _swingSound = "swing_tool";
    [SerializeField]
    private string _poorSound = "hit_poor";
    [SerializeField]
    private AnimationClip _clip;
    [SerializeField, AssetsOnly]
    private GameObject _model;
    [SerializeField]
    private float _stunDuration = 1.2f;
    [SerializeField]
    private float _onSwing = 0.55f;
    [SerializeField, Sirenix.OdinInspector.MinValue("_onSwing")]
    private float _onUse = 0.8f;
    [SerializeField]
    private Vector3 _halfExtents = new Vector3(0.5f, 1.0f, 0.5f);
    [SerializeField]
    private Vector3 _centerOffset = new Vector3(0, 0, 1f);

    PlayerStamina _playerStamina;

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
            Gizmos.DrawWireCube(_centerOffset, _halfExtents * 2);
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
       
        Collider[] colliders = Physics.OverlapBox(context.transform.position + context.transform.rotation * _centerOffset, _halfExtents, context.transform.rotation, LayerMask.NameToLayer("default"));
        List<Collider> usableColliders = new List<Collider>(colliders.Length);

        usableColliders.AddRange(colliders.Where(x => x.GetComponentInParent<IUsable>() != null));
        
        Collider collider = new KdTree<Collider>(usableColliders).FindClosest(context.transform.position);

        if (collider != null)
        {
            if (!context.CallUsable(collider.GetComponentInParent<IUsable>()))
                SoundPlayer.Instance.Play(_poorSound);
        }
    }
}
