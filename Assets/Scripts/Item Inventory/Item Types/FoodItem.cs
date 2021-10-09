using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using MyBox;

public class FoodItem : IUse, IItem
{
    [OdinSerialize]
    public uint ItemStack { get; set; } = 64;
    [OdinSerialize]
    public float Cooldown { get; set; } = 2;

    [SerializeField]
    private float _staminaChange;

    private PlayerStamina _playerStamina;

    void IItem.Initialize(ItemContext context)
    {
        _playerStamina = context.playerTransform.GetComponent<PlayerStamina>();
    }

    void IItem.OnEquip(ItemContext context)
    {

    }

    void IItem.OnUnequip(ItemContext context)
    {

    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {
        if (context.started)
        {
            _playerStamina.Stamina = _staminaChange;
        }

        yield break;
    }
}
