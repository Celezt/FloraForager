using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2 xz(this Vector3 vector) => new Vector2(vector.x, vector.z);
    public static Vector3 xz(this Vector2 vector) => new Vector3(vector.x, 0, vector.y);
    public static Vector3 Vector3X(this float value) => new Vector3(value, 0, 0);
    public static Vector3 Vector3Y(this float value) => new Vector3(0, value, 0);
    public static Vector3 Vector3Z(this float value) => new Vector3(0, 0, value);
}
