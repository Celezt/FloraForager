using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class FishItem : IItem, IStar, IValue
{
    public float Haste => _haste;
    public float Calmness => _calmness;
    public float Randomness => _randomness;
    public AnimationCurve[] UpPatterns => _upPatterns;
    public AnimationCurve[] IdlePatterns => _idlePatterns;
    public AnimationCurve[] DownPatterns => _downPatterns;

    [OdinSerialize, PropertyOrder(-3)]
    int IItem.ItemStack { get; set; } = 16;
    [OdinSerialize, PropertyOrder(-2)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(-1)]
    int IValue.BaseValue { get; set; }

    [Title("Fish behaviour")]
    [SerializeField, Tooltip("Swim frequency."), Range(0, 1)]
    private float _calmness = 0.5f;
    [SerializeField, Tooltip("Swim speed.")]
    private float _haste = 3;
    [SerializeField, Tooltip("Perlin noise multiplier.")]
    private float _randomness = 1;
    [SerializeField, InlineProperty, HideReferenceObjectPicker]
    [Tooltip("swim up patterns. Always starts at y=0.")]
    [ListDrawerSettings(CustomAddFunction = nameof(CustomAddUpPatterns))]
    private AnimationCurve[] _upPatterns = new AnimationCurve[] { CustomAddUpPatterns() };
    [SerializeField, InlineProperty, HideReferenceObjectPicker]
    [Tooltip("Idle patterns. Always starts at y=0.")]
    [ListDrawerSettings(CustomAddFunction = nameof(CustomAddIdlePatterns))]
    private AnimationCurve[] _idlePatterns = new AnimationCurve[] { CustomAddIdlePatterns() };
    [SerializeField, InlineProperty, HideReferenceObjectPicker]
    [Tooltip("swim down patterns. Always starts at y=0.")]
    [ListDrawerSettings(CustomAddFunction = nameof(CustomAddDownPatterns))]
    private AnimationCurve[] _downPatterns = new AnimationCurve[] { CustomAddDownPatterns() };

    private static AnimationCurve CustomAddUpPatterns() => AnimationCurve.EaseInOut(0, 0, 1, 1);
    private static AnimationCurve CustomAddIdlePatterns() => AnimationCurve.Constant(0, 1, 0);
    private static AnimationCurve CustomAddDownPatterns() => AnimationCurve.EaseInOut(0, 0, 1, -1);

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
