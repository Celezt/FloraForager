using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectiveData
{
    [SerializeField] private string _ItemID;
    [SerializeField] private int _Amount;

    public string ItemID => _ItemID;
    public int Amount => _Amount;
}
