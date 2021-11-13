using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

[System.Serializable]
public struct DialoguePriority
{
    public float Priority;
    public AssetReferenceText Dialogue;
    [PropertySpace(5), ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = false, Expanded = true)]
    public string[] Aliases;
}

[System.Serializable]
public struct DialogueElement
{
    public AssetReferenceText Dialogue;
    [PropertySpace(5), ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = false, Expanded = true)]
    public string[] Aliases;
}

[System.Serializable]
public struct DialogueRelation
{
    public Relation AtRelation;
    [ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = true, DraggableItems = false, Expanded = true)]
    public DialoguePriority[] NewDialogue;
}

[System.Serializable]
public struct DialogueEvent
{
    public string NPC;
    [ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = true, DraggableItems = false, Expanded = true)]
    public DialoguePriority[] NewDialogue;
}