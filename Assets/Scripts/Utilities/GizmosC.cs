using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmosC
{
    /// <summary>
    /// Draws a wire arc.
    /// </summary>
    /// <param name="position">World Position.</param>
    /// <param name="dir">The direction from which the anglesRange is taken into account.</param>
    /// <param name="anglesRange">The angle range, in degrees.</param>
    /// <param name="radius"></param>
    /// <param name="maxSteps">How many steps to use to draw the arc.</param>
    public static void DrawWireArc(Vector3 position, Vector3 dir, float radius, float anglesRange = 360, float maxSteps = 20)
    {
        var srcAngles = GetAnglesFromDir(position, dir);
        var initialPos = position;
        var posA = initialPos;
        var stepAngles = anglesRange / maxSteps;
        var angle = srcAngles - anglesRange / 2;

        if (anglesRange >= 360) // Draw first line only when the arc is below 360 degrees.
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            angle += stepAngles;
            posA = posB;
        }

        for (var i = (anglesRange < 360 ? 0 : 1); i <= maxSteps; i++)
        {
            var rad = Mathf.Deg2Rad * angle;
            var posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }

        if (anglesRange < 360)
            Gizmos.DrawLine(posA, initialPos);
    }

    private static float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        var forwardLimitPos = position + dir;
        var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }
}
