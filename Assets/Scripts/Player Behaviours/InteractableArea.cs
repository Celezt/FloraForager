using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IngameDebugConsole;

public class InteractableArea : MonoBehaviour
{
    [SerializeField, Min(0)] private float _radius = 3.0f;
    [SerializeField] private LayerMask _interactableLayer;

    private Vector2 _screenPosition;

    public void OnSelect(InputAction.CallbackContext context)
    {
        Ray ray = Camera.main.ScreenPointToRay(_screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _interactableLayer))
        {
            if (Vector3.Distance(transform.position, hit.point) <= _radius)
            {
                IInteractable[] interactables = hit.transform.gameObject.GetComponents<IInteractable>();
                for (int i = 0; i < interactables.Length; i++)
                {
                    interactables[i].OnInteract(new InteractContext(
                        hit.collider,
                        hit.transform,
                        hit.point,
                        hit.normal,
                        hit.barycentricCoordinate,
                        _screenPosition,
                        hit.textureCoord,
                        hit.textureCoord2,
                        hit.distance,
                        hit.triangleIndex,
                        context.canceled,
                        context.started,
                        context.performed));
                }
            }
        }
    }

    public void OnCursor(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        _screenPosition = value;
    }

    public void SetRadius(float radius) => _radius = radius;

    private void Start()
    {
        DebugLogConsole.AddCommandInstance("player_interact_radius", "Set player's interactable radius", nameof(SetRadius), this);
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (DebugManager.DebugMode)
        {
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            GizmosC.DrawWireArc(transform.position, Vector3.up, _radius, 360, 40);
        }
#endif
    }
}
