using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftableItem", menuName = "Scriptable Objects/Craftable Item")]
public class CraftableItemData : ScriptableObject
{
    [SerializeField] private string _ItemID; // item to give
    [SerializeField] private Sprite _Sprite; 
    [SerializeField] private ResourceRequirement<string, int>[] _ResourceReqs;

    public string ItemID => _ItemID;
    public Sprite Sprite => _Sprite;
    public ResourceRequirement<string, int>[] ResourceReqs => _ResourceReqs;
}

[Serializable]
public struct ResourceRequirement<T1, T2>
{
    public T1 ItemID;
    public T2 Amount;
}
