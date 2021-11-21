using System;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Craftable Item", menuName = "Game Data/Craftable Item")]
public class CraftableItemInfo : SerializedScriptableObject
{
    [Title("Item to craft")]
    [SerializeField, HideLabel, InlineProperty(LabelWidth = 50)] 
    private ItemAsset _Item;
    [Title("Stamina")]
    [SerializeField] 
    private float _StaminaChange = -0.1f; // stamina consumed upon crafting
    [Title("Item requirements")]
    [SerializeField, ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = false, Expanded = true)] 
    private ItemAsset[] _Requirements = new ItemAsset[1];
    
    public ItemAsset Item => _Item;
    public float StaminaChange => _StaminaChange;
    public ItemAsset[] Requirements => _Requirements;
}
