using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 xz(this Vector3 vector) => new Vector2(vector.x, vector.z);
}
