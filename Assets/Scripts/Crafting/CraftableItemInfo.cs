using System;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Craftable Item", menuName = "Game Data/Craftable Item")]
public class CraftableItemInfo : SerializedScriptableObject
{
    [OdinSerialize, ListDrawerSettings(Expanded = true)] 
    private ItemAsset _Item; // item to give
    [OdinSerialize, ListDrawerSettings(Expanded = true)] 
    private ItemAsset[] _Requirements = new ItemAsset[1];
    
    public ItemAsset Item => _Item;
    public ItemAsset[] Requirements => _Requirements;
}
