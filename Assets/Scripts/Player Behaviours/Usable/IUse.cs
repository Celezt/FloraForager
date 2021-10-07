using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUse
{
    public IEnumerable<IUsable> OnUse(UseContext context);
    public void OnUpdate(UseContext context);
    public void OnUnequip(UseContext context);
    public void OnEquip(UseContext context);
}
