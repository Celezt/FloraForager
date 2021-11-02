using System;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Craftable Item", menuName = "Game Data/Craftable Item")]
public class CraftableItemInfo : SerializedScriptableObject
{
    [OdinSerialize] 
    private string _ItemID; // item to give
    [OdinSerialize, ListDrawerSettings(Expanded = true)] 
    private ItemAsset[] _Requirements = new ItemAsset[1];
    
    public string ItemID => _ItemID.ToLower(); // safety check
    public ItemAsset[] Requirements => _Requirements;
}
