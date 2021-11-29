using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Relation
{
    Hated = 0,
    Disliked = 1,
    Neutral = 2,
    Liked = 3,
    Loved = 4
}

[System.Serializable]
public class RelationshipManager
{
    [SerializeField, HideInInspector]
    private float minRelation, maxRelation, curRelation;
    [SerializeField, HideInInspector]
    private Relation relation;
    [SerializeField, HideInInspector]
    private string npcName;

    public Relation Relation => relation;

    public RelationshipManager(string npcName, float minRelation, float maxRelation, float curRelation)
    {
        this.npcName = npcName;
        this.minRelation = minRelation;
        this.maxRelation = maxRelation;
        this.curRelation = curRelation;

        UpdateRelation();
    }

    public void AddRelation(float value)
    {
        curRelation = Mathf.Clamp(curRelation + value, minRelation, maxRelation);
        UpdateRelation();
    }

    public void UpdateRelation()
    {
        float difference = maxRelation - minRelation;

        float temp = curRelation - minRelation;

        float procentage = temp / difference;

        int enumLength = System.Enum.GetNames(typeof(Relation)).Length - 1;
        int enumValue = Mathf.RoundToInt(procentage * enumLength);

        relation = (Relation)enumValue;

        AddDialogue();
    }

    private void AddDialogue()
    {
        NPC npc = NPCManager.Instance?.Get(npcName);

        if (npc == null || npc.DialogueRelations == null)
            return;

        for (int i = npc.DialogueRelations.Length - 1; i >= 0; --i)
        {
            DialogueRelationSave dialogueRelation = npc.DialogueRelations[i];

            if ((int)relation != (int)dialogueRelation.AtRelation)
                continue;

            if (!string.IsNullOrWhiteSpace(dialogueRelation.RepeatingDialogue.Item1))
                npc.SetRepeatingDialouge(dialogueRelation.RepeatingDialogue.Item1, dialogueRelation.RepeatingDialogue.Item2);

            if (dialogueRelation.AddedDialogue != null)
            {
                foreach ((float, string, string[]) dialogue in dialogueRelation.AddedDialogue)
                {
                    if (string.IsNullOrWhiteSpace(dialogue.Item2))
                        continue;

                    npc.DialogueQueue.Enqueue((dialogue.Item2, dialogue.Item3), dialogue.Item1);
                }
                dialogueRelation.AddedDialogue = null;
            }

            npc.DialogueRelations[i] = dialogueRelation;
        }
    }
}
