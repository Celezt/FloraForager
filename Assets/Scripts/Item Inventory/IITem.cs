using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    public void OnInitialize(ItemTypeContext context);
    public void OnUpdate(ItemContext context);
    public void OnUnequip(ItemContext context);
    public void OnEquip(ItemContext context);
}
