using System;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Craftable Item", menuName = "Game Data/Craftable Item")]
public class CraftableItemInfo : SerializedScriptableObject
{
    [Title("Item to craft")]
    [OdinSerialize, HideLabel, InlineProperty(LabelWidth = 50)] 
    private ItemAsset _Item; // item to give
    [Title("Item requirements")]
    [OdinSerialize, ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = false, Expanded = true)] 
    private ItemAsset[] _Requirements = new ItemAsset[1];
    
    public ItemAsset Item => _Item;
    public ItemAsset[] Requirements => _Requirements;
}
