using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using MyBox;
using Sirenix.OdinInspector;

public class FoodItem : IUse, IStar, IValue
{
    [OdinSerialize, PropertyOrder(float.MinValue)]
    public int ItemStack { get; set; } = 64;
    [OdinSerialize, PropertyOrder(float.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(float.MinValue + 2)]
    int IValue.BaseValue { get; set; }
    [OdinSerialize, PropertyOrder(float.MinValue + 3)]
    public float Cooldown { get; set; } = 2;

    [SerializeField]
    private float _staminaChange;
    [SerializeField, InlineProperty]
    private List<IEffect> _effects = new List<IEffect>(); 

    private PlayerStamina _playerStamina;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
        _playerStamina = context.transform.GetComponent<PlayerStamina>();
    }

    void IItem.OnUnequip(ItemContext context)
    {
        
    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerator IUse.OnUse(UseContext context)
    {
        if (context.started)
        {
            _playerStamina.Stamina += _staminaChange;

            foreach (IEffect effect in _effects)
                if (!effect.Duration.IsActive)
                    effect.OnEffect(context);

            context.Consume();
        }

        yield break;
    }
}
