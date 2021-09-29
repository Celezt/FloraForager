using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information from the interaction.
/// </summary>
public readonly struct InteractContext
{
    public readonly Transform playerTransform;
    public readonly Vector3 worldPosition;
    public readonly Vector3 normal;
    public readonly Vector3 barycentricCoordinate;
    public readonly Vector3 direction;
    public readonly Vector2 screenPosition;
    public readonly Vector2 textureCoord;
    public readonly Vector2 textureCoord2;
    public readonly float distance;
    public readonly float rayDistance;
    public readonly int triangleIndex;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal InteractContext(
        Transform playerTransform,
        Vector3 worldPosition,
        Vector3 normal,
        Vector3 barycentricCoordinate,
        Vector3 direction,
        Vector2 screenPosition, 
        Vector2 textureCoord,
        Vector2 textureCoord2,
        float distance,
        float rayDistance,
        int triangleIndex,
        int playerIndex,
        bool canceled, 
        bool started,
        bool performed)
    {
        this.playerTransform = playerTransform;
        this.worldPosition = worldPosition;
        this.normal = normal;
        this.barycentricCoordinate = barycentricCoordinate;
        this.direction = direction;
        this.screenPosition = screenPosition;
        this.textureCoord = textureCoord;
        this.textureCoord2 = textureCoord2;
        this.distance = distance;
        this.rayDistance = rayDistance;
        this.triangleIndex = triangleIndex;
        this.playerIndex = playerIndex;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }
}
