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
    private float minRelation, maxRelation;
    private float curRelation;

    private Relation relation;

    public Relation Relation => relation;

    public RelationshipManager(float minRelation, float maxRelation, float curRelation)
    {
        this.minRelation = minRelation;
        this.maxRelation = maxRelation;
        this.curRelation = curRelation;

        UpdateEnum();
    }

    public void AddRelation(float value)
    {
        curRelation = Mathf.Clamp(curRelation + value, minRelation, maxRelation);

        UpdateEnum();
    }

    private void UpdateEnum()
    {
        float difference = maxRelation - minRelation;

        float temp = curRelation - minRelation;

        float procentage = temp / difference;

        int enumLength = System.Enum.GetNames(typeof(Relation)).Length - 1;
        int enumValue = Mathf.RoundToInt(procentage * enumLength);

        relation = (Relation)enumValue;
    }
}
