using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Relation
{
    Loved,
    Liked,
    Neutral,
    Disliked,
    Hated,
}

public class RelationshipManager : MonoBehaviour
{

    private Relation relation;
    [SerializeField] float minRelation = -100, maxRelation = 100;

    [SerializeField] float curRelation;

    public Relation Relation => relation;

    public void AddRelation(float value)
    {
        curRelation += value;

        float difference = maxRelation - minRelation;

        float temp = curRelation - minRelation;

        float procentage = temp / difference;

        int enumLength = System.Enum.GetNames(typeof(Relation)).Length - 1;
        int enumValue = Mathf.RoundToInt(procentage * enumLength);

        relation = (Relation)enumValue;
    }
}