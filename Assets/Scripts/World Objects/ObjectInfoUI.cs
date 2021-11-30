using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectInfoUI : MonoBehaviour
{
    [SerializeField]
    private LayerMask _LayerMasks;
    [SerializeField]
    private float _MaxHitDistance = 5.0f;
    [SerializeField] 
    private float _HeightOffset = 0.5f;
    [SerializeField]
    private string _EnterSound = "enter_button";

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private GameObject _InteractableObject;
    private Bounds _ObjectBounds;

    private GameObject _Player;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = GetComponentInParent<Canvas>().rootCanvas;
        _CanvasRect = _Canvas.GetComponent<RectTransform>();

        _Player = PlayerInput.GetPlayerByIndex(0).gameObject;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, 50f, _LayerMasks) && !CanvasUtility.IsPointerOverUIElement();

        float distance = Mathf.Sqrt(
            Mathf.Pow(hitInfo.point.x - _Player.transform.position.x, 2) +
            Mathf.Pow(hitInfo.point.z - _Player.transform.position.z, 2));

        if (collision && distance <= _MaxHitDistance && (hitInfo.transform.GetComponent(typeof(IInteractable)) as IInteractable) != null)
        {
            if (_InteractableObject == null)
            {
                Show(hitInfo.transform.gameObject);
            }
        }
        else
            Hide();
    }

    private void LateUpdate()
    {
        if (_InteractableObject == null)
            return;

        UpdatePosition();
    }

    private void Show(GameObject interactable)
    {
        _InteractableObject = interactable;

        if (interactable.TryGetComponent(out Collider collider))
            _ObjectBounds = collider.bounds;

        SoundPlayer.Instance.Play(_EnterSound, 0.75f);

        _CanvasGroup.alpha = 1.0f;
    }

    private void Hide()
    {
        _InteractableObject = null;
        _CanvasGroup.alpha = 0.0f;
    }

    private void UpdatePosition()
    {
        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _InteractableObject.transform.position + Vector3.up * (_ObjectBounds.size.y + _HeightOffset));
    }
}
