using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using UnityEditor;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "New ItemType", menuName = "Inventory/ItemType")]
public class ItemType : SerializedScriptableObject
{
    [PreviewField(120), HideLabel]
    [HorizontalGroup("Group 1", 120)]
    public Sprite Icon;
    [VerticalGroup("Group 1/Right"), LabelWidth(80)]
    public string Name;
    [VerticalGroup("Group 1/Right"), LabelWidth(80), ReadOnly]
    public string ID;
    [VerticalGroup("Group 1/Right"), TextArea(5, 30)]
    public string Description;
    [Required, OdinSerialize, HideLabel]
    [ListDrawerSettings(Expanded = true)]
    public IItem Reference;
    public string[] Labels;

#if UNITY_EDITOR
    public void Awake()
    {
        ID = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
    }
#endif
}
