using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CellData
{
    [HorizontalGroup("Group")]
    [VerticalGroup("Group/Left"), LabelWidth(70), SceneObjectsOnly]
    public GameObject HeldObject;
    [VerticalGroup("Group/Right"), LabelWidth(30)]
    public CellType Type;

    public CellData(GameObject heldObject, CellType type = CellType.Undefined)
    {
        HeldObject = heldObject;
        Type = type;
    }
}
