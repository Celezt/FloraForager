using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class SeedItem : IUse
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 32;
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    float IUse.Cooldown { get; set; } = 0.05f;

    [Title("Seed Behaviour")]
    [SerializeField, AssetSelector(Paths = "Assets/Data/Flora"), AssetsOnly]
    private FloraInfo _Flora;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

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
