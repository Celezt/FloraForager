using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Sirenix.OdinInspector;

[System.Serializable]
public struct DialoguePriority
{
    public float Priority;
    public AssetReferenceText Asset;
    [PropertySpace(5), ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = false, Expanded = true)]
    public string[] Aliases;
}

[System.Serializable]
public struct DialogueElement
{
    public AssetReferenceText Asset;
    [PropertySpace(5), ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = false, Expanded = true)]
    public string[] Aliases;
}

[System.Serializable]
public struct DialogueRelation
{
    public Relation AtRelation;
    public DialogueElement RepeatingDialogue;
    [ListDrawerSettings(AlwaysAddDefaultValue = true, ShowItemCount = true, DraggableItems = false, Expanded = true)]
    public DialoguePriority[] AddedDialogue;
}

[System.Serializable]
public struct DialogueRelationSave
{
    public Relation AtRelation;
    public (string, string[]) RepeatingDialogue;
    public (float, string, string[])[] AddedDialogue;
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
    public DialogueElement Dialogue;
}