using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class RodItem : IItem, IUse, IStar, IValue
{
    public float CatchSize => _catchSize;
    public float DragForce => _dragForce;
    public float DragDamp => _dragDamp;
    public float DragWeight => _weight;
    public float Bounciness => _bounciness;

    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 1;
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    float IUse.Cooldown { get; set; } = 1;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    Stars IStar.Star { get; set; }
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    int IValue.BaseValue { get; set; }

    [Title("Rod Behaviour")]
    [SerializeField, Range(0, 1)]
    private float _catchSize = 0.2f;
    [SerializeField]
    private float _weight = 6.0f;
    [SerializeField]
    private float _dragForce = 100.0f;
    [SerializeField]
    private float _dragDamp = 10.0f;
    [SerializeField]
    private float _bounciness = 0.5f;

    void IItem.OnInitialize(ItemTypeContext context)
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
