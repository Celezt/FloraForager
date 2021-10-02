using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public struct FoodItem : IUse, IItem, IStamina
{
    [OdinSerialize]
    public uint ItemStack { get; set; }

    [OdinSerialize]
    public float StaminaChange { get; set; }


    public void OnActive(UseContext context)
    {

    }

    public void OnInactive(UseContext context)
    {

    }

    public void OnUpdate(UseContext context)
    {

    }

    public void OnUse(UseContext context)
    {

    }
}
