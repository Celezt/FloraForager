using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public uint ItemStack { get; set; }

    public void OnUpdate(ItemContext context);
    public void OnUnequip(ItemContext context);
    public void OnEquip(ItemContext context);
}
