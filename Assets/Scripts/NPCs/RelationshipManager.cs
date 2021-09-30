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
    int RMeter;

    private Relation relation;
    [SerializeField] float minRelation, maxRelation;

    [SerializeField] float curRelation;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            addRelation(10);
        }
    }

    public void addRelation(float value)
    {
        curRelation += value;

        float difference = maxRelation - minRelation;

        float temp = curRelation - minRelation;

        float procentage = temp / difference;

        int enumValue = (int)Mathf.Round(procentage * 5.0f);

        relation = (Relation)enumValue;

        Debug.Log("The current relation is.. " + relation);
        Debug.Log(enumValue + ", " + difference + ", " + temp + ", " + procentage);
    }
}
