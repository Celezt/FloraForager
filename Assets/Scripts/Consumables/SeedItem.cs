using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class SeedItem : IUse, IItem
{
    [OdinSerialize]
    public int ItemStack { get; set; } = 64;
    [OdinSerialize]
    public float Cooldown { get; set; } = 2;

    [OdinSerialize, Required] 
    private string _FloraName;

    private Inventory _Inventory;

    public void OnEquip(ItemContext context)
    {
        _Inventory = context.transform.GetComponent<PlayerInfo>().Inventory;
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

        FloraMaster.Instance.Add(_FloraName.ToLower());

        yield break;
    }
}
