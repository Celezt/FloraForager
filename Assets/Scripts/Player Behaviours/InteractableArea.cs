using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IngameDebugConsole;

public class InteractableArea : MonoBehaviour
{
    [SerializeField] SchemeManager _schemeManager;
    [SerializeField, Min(0)] private float _radius = 3.0f;
    [SerializeField] private LayerMask _interactableLayers;

    private Vector2 _screenPosition;

    private PlayerAction _inputs;

    private struct InteractableObject
    {
        public Collider Collider { get; set; }
        public IInteractable Interactable { get; set; }
        public Vector3 Position { get; set; }
        public int Priority { get; set; }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        void InteractAtCursor(InputAction.CallbackContext context)
        {
            Ray ray = Camera.main.ScreenPointToRay(_screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _interactableLayers))
            {
                if (Vector3.Distance(transform.position, hit.point) <= _radius)
                {
                    IInteractable interactable = hit.transform.gameObject.GetComponent<IInteractable>();

                    if (interactable == null)
                        return;

                    interactable.OnInteract(new InteractContext(
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

        void InteractAtClosest(InputAction.CallbackContext context)
        {
            Vector3 position = transform.position;
            Collider[] colliders = Physics.OverlapSphere(position, _radius, _interactableLayers);
            List<InteractableObject> interactableObjects = new List<InteractableObject>(colliders.Length);

            int highestPriority = 0;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject == gameObject)  // Skip itself.
                    continue;

                if (colliders[i].TryGetComponent(out IInteractable interactable))
                {
                    int priority = interactable.Priority;
                    // Add all component with IInteractable.
                    interactableObjects.Add(new InteractableObject {
                        Collider = colliders[i],
                        Interactable = interactable, 
                        Position = colliders[i].transform.position, 
                        Priority = priority
                    });

                    if (priority > highestPriority)
                        highestPriority = priority;
                }
            }

            InteractableObject interactableObject = new InteractableObject();
            float minDistance = _radius;
            bool foundAny = false;
            for (int i = 0; i < interactableObjects.Count; i++)
            {
                if (interactableObjects[i].Priority == highestPriority) // Only loop through the highest priority interactables.
                {
                    float distance = Vector3.Distance(interactableObjects[i].Position, position);

                    if (distance < minDistance) // Get the current closest interactable.
                    {
                        interactableObject = interactableObjects[i];
                        minDistance = distance;
                        foundAny = true;
                    }
                }
            }

            if (!foundAny)  // Return if no interactable objects could be found.
                return;

            // Raycast artificial ray to the closest interactable object.
            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(interactableObject.Position));
            interactableObject.Collider.Raycast(ray, out RaycastHit hit, Mathf.Infinity);

            interactableObject.Interactable.OnInteract(new InteractContext(
                interactableObject.Collider,
                interactableObject.Collider.transform,
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

        if (_schemeManager.CurrentScheme == _inputs.KeyboardAndMouseScheme)
            InteractAtCursor(context);
        else if (_schemeManager.CurrentScheme == _inputs.Dualshock4Scheme)
            InteractAtClosest(context);
    }

    public void OnCursor(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();

        _screenPosition = value;
    }

    public void SetRadius(float radius) => _radius = radius;

    private void Awake()
    {
        _inputs = new PlayerAction();
    }

    private void Start()
    {
        DebugLogConsole.AddCommandInstance("player_interact_radius", "Set player's interactable radius", nameof(SetRadius), this);
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Ground.Interact.started += OnInteract;
        _inputs.Ground.Interact.performed += OnInteract;
        _inputs.Ground.Interact.canceled += OnInteract;
        _inputs.Ground.Cursor.started += OnCursor;
        _inputs.Ground.Cursor.performed += OnCursor;
        _inputs.Ground.Cursor.canceled += OnCursor;
    }

    private void OnDisable()
    {
        _inputs.Disable();
        _inputs.Ground.Interact.started -= OnInteract;
        _inputs.Ground.Interact.performed -= OnInteract;
        _inputs.Ground.Interact.canceled -= OnInteract;
        _inputs.Ground.Cursor.started -= OnCursor;
        _inputs.Ground.Cursor.performed -= OnCursor;
        _inputs.Ground.Cursor.canceled -= OnCursor;
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
