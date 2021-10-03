using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public enum Relation
{
    Hated,
    Disliked,
    Neutral,
    Liked,
    Loved
}

public class RelationshipManager : MonoBehaviour
{

    private Relation relation;
    [SerializeField] MinMaxFloat RelationsRange = new MinMaxFloat(-100, 100);

    [SerializeField] float curRelation;

    public Relation Relation => relation;

    public void AddRelation(float value)
    {
        curRelation += value;
        curRelation = Mathf.Clamp(curRelation, RelationsRange.Min, RelationsRange.Max);

        float difference = RelationsRange.Length();

        float temp = curRelation - RelationsRange.Min;

        float procentage = temp / difference;

        int enumLength = System.Enum.GetNames(typeof(Relation)).Length - 1;
        int enumValue = Mathf.RoundToInt(procentage * enumLength);

        relation = (Relation)enumValue;
    }
}
