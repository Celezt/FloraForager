using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUse
{
    public IEnumerable<IUsable> OnUse(UseContext context);
}
