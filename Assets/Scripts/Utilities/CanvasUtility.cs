using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class CanvasUtility
{
    public static Vector2 WorldToCanvasPosition(Canvas canvas, RectTransform canvasRect, Camera camera, Vector3 worldPosition)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : camera, out Vector2 result);

        return canvas.transform.TransformPoint(result);
    }

    public static bool IsPointerOverUIElement() => IsPointerOverUIElement(GetEventSystemRaycastResults());

    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }

        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
