using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructableObject : IUsable
{
    /// <summary>
    /// When taking damage.
    /// </summary>
    /// <param name="destructor">Damage source.</param>
    /// <param name="destructable">This.</param>
    public void OnDamage(IDestructor destructor, IDestructable destructable, UsedContext context);

    /// <summary>
    /// When destroyed.
    /// </summary>
    public void OnDestruction(UsedContext context);
}
