using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using UnityEditor;

[CreateAssetMenu(fileName = "LabelData", menuName = "Inventory/LabelData")]
public class LabelSettings : ScriptableObject
{
    [SerializeField]
    public List<string> Labels = new List<string>();
}
