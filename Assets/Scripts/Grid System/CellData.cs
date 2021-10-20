using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CellData
{
    public CellType Type;

    public CellData(CellType type = CellType.Undefined)
    {
        Type = type;
    }
}
