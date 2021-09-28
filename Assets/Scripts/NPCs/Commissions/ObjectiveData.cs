using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectiveData
{
    [SerializeField] private string _Type;
    [SerializeField] private int _Amount;

    public string Type => _Type;
    public int Amount => _Amount;
}
