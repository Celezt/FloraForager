using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUse : IItem
{
    public float Cooldown { get; set; }
    public IEnumerator OnUse(UseContext context);
}
