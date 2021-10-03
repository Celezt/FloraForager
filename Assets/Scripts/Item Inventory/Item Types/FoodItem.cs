using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using MyBox;

public struct FoodItem : IUse, IItem, IStamina
{
    [OdinSerialize]
    public uint ItemStack { get; set; }

    [OdinSerialize]
    public float StaminaChange { get; set; }

    public void OnEquip(UseContext context)
    {

    }

    public void OnUnequip(UseContext context)
    {

    }

    public void OnUpdate(UseContext context)
    {

    }

    public void OnUse(UseContext context)
    {

    }
}
