using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class FloraInfoUI : MonoBehaviour
{
    [SerializeField] private SpriteAtlas _WaterDropAtlas;
    [SerializeField] private TMP_Text _Name;
    [SerializeField] private TMP_Text _Status;
    [SerializeField] private TMP_Text _Stage;
    [SerializeField] private Image _WateredImage;
    [SerializeField] private float _HeightOffset = 0.5f;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private FloraObject _FloraObject;
    private Bounds _FloraBounds;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = transform.root.GetComponent<Canvas>();
        _CanvasRect = _Canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell != null && cell.HeldObject != null)
        {
            if (_FloraObject == null || _FloraObject != cell.HeldObject)
            {
                if (cell.HeldObject.TryGetComponent(out FloraObject flora))
                {
                    Show(flora);
                }
            }
        }
        else
            Hide();
    }

    private void LateUpdate()
    {
        if (_FloraObject == null)
            return;

        UpdatePosition();
        UpdateWindow();
    }

    private void Show(FloraObject flora)
    {
        _FloraObject = flora;

        if (flora.TryGetComponent(out MeshFilter meshFilter))
            _FloraBounds = meshFilter.mesh.bounds;

        UpdateWindow();

        _CanvasGroup.alpha = 1.0f;
    }

    private void Hide()
    {
        _FloraObject = null;
        _FloraBounds = new Bounds();

        _CanvasGroup.alpha = 0.0f;
    }

    private void UpdatePosition()
    {
        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _FloraObject.transform.position + Vector3.up * _FloraBounds.size.y * _HeightOffset);
    }

    private void UpdateWindow()
    {
        _Name.text = _FloraObject.Flora.Info.Name;
        _Status.text = GetStatus();
        _Stage.text = _FloraObject.Flora.FloraData.Stage.ToString();

        int watered = _FloraObject.Flora.FloraData.Watered ? 1 : 0;
        _WateredImage.sprite = _WaterDropAtlas.GetSprite($"water-drop_{watered}");
    }

    private string GetStatus()
    {
        if (_FloraObject.Flora.Completed)
            return "Done";
        else if (_FloraObject.Flora.FloraData.Watered)
            return "Growing";
        else 
            return "Idle";
    }
}
