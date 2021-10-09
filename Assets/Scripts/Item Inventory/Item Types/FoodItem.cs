using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using MyBox;

public class FoodItem : IUse, IItem, IStamina
{
    [OdinSerialize]
    public uint ItemStack { get; set; } = 64;
    [OdinSerialize]
    public float Cooldown { get; set; } = 2;

    [SerializeField]
    private float _staminaChange;

    float IStamina.OnStaminaChange(float currentStamina) => currentStamina + _staminaChange;

    void IItem.Initialize(ItemContext context)
    {
       
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
        yield break;
    }
}
