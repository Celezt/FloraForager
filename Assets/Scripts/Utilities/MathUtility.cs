using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathUtility
{
    /// <summary>
    /// Checks if given point is inside of arc
    /// </summary>
    /// <param name="arc">in radians</param>
    /// <returns>true if point is inside of arc</returns>
    public static bool PointInArc(Vector3 point, Vector3 position, float directionDegrees, float arcDegrees, float radius)
    {
        float range = Mathf.Sqrt(
            Mathf.Pow(point.x - position.x, 2) +
            Mathf.Pow(point.z - position.z, 2));
        float angle = Mathf.Atan2(
            point.z - position.z,
            point.x - position.x) * Mathf.Rad2Deg;

        if (angle < 0.0f)
            angle += 360.0f;

        arcDegrees /= 2.0f;

        float startAngle = (90.0f - directionDegrees - arcDegrees + 360.0f) % 360.0f;
        float endAngle = (90.0f - directionDegrees + arcDegrees + 360.0f) % 360.0f;

        if (range > radius)
            return false;
        else if (startAngle < endAngle)
            return angle > startAngle && angle < endAngle;
        else if (startAngle >= endAngle)
            return angle > startAngle || angle < endAngle;

        return false;
    }
}
