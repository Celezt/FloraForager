using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasUtility
{
    public static Vector2 WorldToCanvasPosition(Canvas canvas, RectTransform canvasRect, Camera camera, Vector3 worldPosition)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
        Vector2 result = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : camera, out result);

        return canvas.transform.TransformPoint(result);
    }
}
