using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestructable : IResource
{
    public float Durability { get; set; }
}
