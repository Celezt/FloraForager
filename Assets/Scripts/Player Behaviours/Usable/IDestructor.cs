using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructor : IUse
{
    public float Damage { get; set; }
}
