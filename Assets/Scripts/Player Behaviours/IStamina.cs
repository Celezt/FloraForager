using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStamina
{
    /// <summary>
    /// Modify player's stamina.
    /// </summary>
    /// <param name="currentStamina">Current player stamina.</param>
    /// <returns>Changed stamina.</returns>
    public float OnStaminaChange(float currentStamina);
}
