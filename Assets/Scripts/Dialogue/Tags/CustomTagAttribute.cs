using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Create custom tag to be used in dialogue.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public class CustomTagAttribute : Attribute
{

}
