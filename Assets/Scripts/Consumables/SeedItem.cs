using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class SeedItem : IUse, IItem
{
    [OdinSerialize]
    public uint ItemStack { get; set; } = 64;
    [OdinSerialize]
    public float Cooldown { get; set; } = 2;

    [OdinSerialize]
    private FloraData _Flora;

    public void OnEquip(ItemContext context)
    {

    }

    public void OnUnequip(ItemContext context)
    {

    }

    public void OnUpdate(ItemContext context)
    {

    }

    public IEnumerable<IUsable> OnUse(UseContext context)
    {
        if (!context.performed)
            yield break;

        FloraMaster.Instance.Add(_Flora);

        context.Consume();

        yield break;
    }
}
