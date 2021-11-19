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
    [SerializeField] private float _HeightOffset = 0.75f;

    private CanvasGroup _CanvasGroup;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private FloraObject _FloraObject;
    private float _FloraHeightOffset;

    private Cell _DirtCell;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
        _CanvasGroup.alpha = 0.0f;

        _Canvas = GetComponentInParent<Canvas>().rootCanvas;
        _CanvasRect = _Canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Cell cell = Grid.Instance.HoveredCell;

        if (cell != null && cell.Type == CellType.Dirt)
        {
            _DirtCell = cell;

            if (_DirtCell.HeldObject == null)
                Show(null);
            else if (_DirtCell.HeldObject != _FloraObject && _DirtCell.HeldObject.TryGetComponent(out FloraObject floraObject))
                Show(floraObject);
        }
        else
            Hide();
    }

    private void LateUpdate()
    {
        if (_DirtCell == null)
            return;

        UpdatePosition();
        UpdateWindow();
    }

    private void Show(FloraObject flora)
    {
        _FloraObject = flora;

        if (_FloraObject != null && _FloraObject.TryGetComponent(out MeshFilter meshFilter))
            _FloraHeightOffset = meshFilter.mesh.bounds.extents.y;
        else
            _FloraHeightOffset = 0.0f;

        UpdateWindow();

        _CanvasGroup.alpha = 1.0f;
    }

    private void Hide()
    {
        _FloraObject = null;
        _FloraHeightOffset = 0.0f;

        _DirtCell = null;

        _CanvasGroup.alpha = 0.0f;
    }

    private void UpdatePosition()
    {
        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _DirtCell.Middle + Vector3.up * (_FloraHeightOffset + _HeightOffset));
    }

    private void UpdateWindow()
    {
        if (_FloraObject != null)
        {
            _Name.text = _FloraObject.Flora.Info.Name;
            _Status.text = GetStatus();
            _Stage.text = _FloraObject.Flora.FloraData.Stage.ToString();

            int watered = _FloraObject.Flora.FloraData.Watered ? 1 : 0;
            _WateredImage.sprite = _WaterDropAtlas.GetSprite($"water-drop_{watered}");
        }
        else
        {
            _Name.text = "Empty";
            _Status.text = "Idle";
            _Stage.text = "0";
            _WateredImage.sprite = _WaterDropAtlas.GetSprite($"water-drop_{0}");
        }
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
