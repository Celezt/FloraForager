using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlacedContext
{
    public readonly IItem item;
    public readonly List<string> labels;
    public readonly UseBehaviour behaviour;
    public readonly Transform transform;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;

    internal PlacedContext(
    IItem item,
    List<string> labels,
    Transform transform,
    UseBehaviour behaviour,
    string name,
    string id,
    int playerIndex)
    {
        this.item = item;
        this.labels = labels;
        this.transform = transform;
        this.behaviour = behaviour;
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
    }
}
