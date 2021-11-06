using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 xz(this Vector3 vector) => new Vector2(vector.x, vector.z);
    public static Vector3 xz(this Vector2 vector) => new Vector3(vector.x, 0, vector.y);
}
