using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructable : IResource
{
    public float Strength { get; set; }
    public float Durability { get; set; }
}
