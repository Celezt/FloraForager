using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class DebugInteractableArea : MonoBehaviour, IInteractable
{
    [SerializeField] private int _priority = 0;

    public int Priority => _priority;

    public void OnInteract(InteractContext context)
    {
        if (context.performed)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sphere.transform.position = context.worldPosition;
        }
    }
}
#endif
