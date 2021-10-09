using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUse
{
    public float Cooldown { get; set; }
    public IEnumerable<IUsable> OnUse(UseContext context);
}
