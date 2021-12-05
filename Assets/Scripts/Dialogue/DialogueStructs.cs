using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

[System.Serializable]
public struct DialoguePriority
{
    [HorizontalGroup("Group")]
    [VerticalGroup("Group/Left"), LabelWidth(50)]
    public float Priority;
    [HideLabel, InlineProperty, VerticalGroup("Group/Right")]
    public AssetReferenceText Dialogue;
}

[System.Serializable]
public struct DialogueRelation
{
    public Relation AtRelation;
    public AssetReferenceText RepeatingDialogue;
    [ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = true, DraggableItems = false, Expanded = true)]
    public DialoguePriority[] AddedDialogue;
}

[System.Serializable]
public struct DialogueRelationSave
{
    public Relation AtRelation;
    public string RepeatingDialogue;
    public (float, string)[] AddedDialogue;
}

[System.Serializable]
public struct DialogueEvent
{
    public string NPC;
    [ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = true, DraggableItems = false, Expanded = true)]
    public DialoguePriority[] AddedDialogue;
}

[System.Serializable]
public struct DialogueAction
{
    public string NPC;
    [HideLabel, InlineProperty]
    public AssetReferenceText Dialogue;
}