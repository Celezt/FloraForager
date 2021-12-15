using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class FishBaitItem : IItem, IBait, IStar
{
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;

    [Title("Bait Behaviour")]
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    float IBait.Efficiency { get; set; } = 5;

    [SerializeField]
    private CellType[] _allowedUse = new CellType[] { CellType.Water };

    public CellType[] AllowedUse => _allowedUse;

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
}
