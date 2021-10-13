using System;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "CraftableItem", menuName = "Scriptable Objects/Craftable Item")]
public class CraftableItemData : SerializedScriptableObject
{
    [OdinSerialize] 
    private string _ItemID; // item to give
    [OdinSerialize] 
    private ResourceRequirement[] _ResourceReqs = new ResourceRequirement[1];

    public string ItemID => _ItemID.ToLower(); // safety check
    public ResourceRequirement[] ResourceReqs => _ResourceReqs;

}
