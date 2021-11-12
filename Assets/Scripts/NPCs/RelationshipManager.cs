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

    private void UpdateRelation()
    {
        float difference = maxRelation - minRelation;

        float temp = curRelation - minRelation;

        float procentage = temp / difference;

        int enumLength = System.Enum.GetNames(typeof(Relation)).Length - 1;
        int enumValue = Mathf.RoundToInt(procentage * enumLength);

        relation = (Relation)enumValue;

        NPC npc = NPCManager.Instance?.Get(npcName);

        if (npc == null)
            return;

        for (int i = npc.RelationDialogue.Count - 1; i >= 0; --i)
        {
            DialogueRelation dialogueRelation = npc.RelationDialogue[i];
            if ((int)relation == (int)dialogueRelation.AtRelation)
            {
                foreach (DialoguePriority dialogue in dialogueRelation.NewDialogue)
                {
                    npc.DialogueQueue.Enqueue((dialogue.Dialogue.AssetGUID, dialogue.Aliases), dialogue.Priority);
                }
                npc.RelationDialogue.RemoveAt(i);
            }
        }
    }
}
