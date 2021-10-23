using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CellData
{
    [HorizontalGroup("Group")]
    [HorizontalGroup("Group/Left", LabelWidth = 80, Width = 260)]
    public GameObject HeldObject;
    [HorizontalGroup("Group/Right", LabelWidth = 30, Width = 130)]
    public CellType Type;

    public CellData(GameObject heldObject, CellType type = CellType.Undefined)
    {
        HeldObject = heldObject;
        Type = type;
    }
}
