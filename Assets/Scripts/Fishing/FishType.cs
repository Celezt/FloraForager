using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class FishType : IItem
{
    [OdinSerialize]
    uint IItem.ItemStack { get; set; } = 64;

    private struct Fish
    {
        [Tooltip("Preferred height [0,1]"), Range(0, 1)]
        public float Height;
        public float Haste;
        public float Calmness;
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
