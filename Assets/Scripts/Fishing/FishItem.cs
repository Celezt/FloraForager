using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class FishItem : IItem
{
    public FishData Data => _fish;

    [OdinSerialize]
    uint IItem.ItemStack { get; set; } = 64;

    [SerializeField]
    private FishData _fish;

    [Serializable, HideLabel]
    public struct FishData
    {
        [Tooltip("Preferred height [0,1]."), Range(0, 1)]
        public float Height;
        [Tooltip("Swim speed."), Min(0)]
        public float Haste;
        [Tooltip("Swim frequency."), Min(0)]
        public float Calmness;
        [Tooltip("Randomness multiplier.")]
        public float Randomness;
    }

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
}
