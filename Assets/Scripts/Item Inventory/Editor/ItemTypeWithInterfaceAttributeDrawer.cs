using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using System;

[CustomPropertyDrawer(typeof(ItemTypeWithInterfaceAttribute))]
public class ItemTypeWithInterfaceAttributeDrawer : PropertyDrawer
{
    private Type T;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            ItemTypeWithInterfaceAttribute requiredAttribute = attribute as ItemTypeWithInterfaceAttribute;

            T = requiredAttribute.RequiredType;

            UnityEngine.Object lastValidRef = property.objectReferenceValue;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();
            ItemType temp = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(IResource), false) as ItemType;

            if (EditorGUI.EndChangeCheck())
            {
                if (temp != null)
                {
                    if (temp.Behaviour != null && requiredAttribute.RequiredType.IsAssignableFrom(temp.Behaviour.GetType()))
                        property.objectReferenceValue = temp;
                    else
                        property.objectReferenceValue = lastValidRef;
                }
                else
                    property.objectReferenceValue = null;
            }

            EditorGUI.EndProperty();
        }
    }
}
