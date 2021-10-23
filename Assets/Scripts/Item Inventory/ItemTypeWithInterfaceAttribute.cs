using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ItemTypeWithInterfaceAttribute : PropertyAttribute
{
    public Type RequiredType;

    public ItemTypeWithInterfaceAttribute(Type type)
    {
        RequiredType = type;
    }
}
