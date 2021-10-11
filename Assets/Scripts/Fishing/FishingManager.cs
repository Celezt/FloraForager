using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IngameDebugConsole;
using MyBox;

public class FishingManager : MonoBehaviour
{
    private static Dictionary<int, FishingManager> _fishings = new Dictionary<int, FishingManager>();

    [SerializeField] private RectTransform _fishArea;
    [SerializeField] private RectTransform _catchArea;
    [SerializeField] private RectTransform _fishPoint;
    [SerializeField, Min(0)] private int _fishHeight = 500;
    [SerializeField, Min(0)] private int _playerIndex;

    private Coroutine _playFishingCoroutine;
    private PlayerAction _inputs;

    private float _drag;

    /// <summary>
    /// Return Fishing Manager based on the player index connected to it.
    /// </summary>
    /// <param name="playerIndex">Index.</param>
    /// <returns>Fishing Manager.</returns>
    public static FishingManager GetByIndex(int playerIndex) => _fishings[playerIndex];

    public void OnDrag(InputAction.CallbackContext context)
    {
        _drag = context.ReadValue<float>();
    }

    public void StartFishing(string fishID, string rodID)
    {
        if (!ItemTypeSettings.Instance.ItemTypeChunk.ContainsKey(fishID))
        {
            Debug.LogWarning($"{fishID} does not exist.");
            return;
        }

        ItemType item = ItemTypeSettings.Instance.ItemTypeChunk[fishID];

        if (!(item.Behaviour is FishItem))
        {
            Debug.LogWarning($"{fishID} is not a fish.");
            return;
        }

        if (!ItemTypeSettings.Instance.ItemTypeChunk.ContainsKey(rodID))
        {
            Debug.LogWarning($"{rodID} does not exist.");
            return;
        }

        RodItem rodItem = (RodItem)ItemTypeSettings.Instance.ItemTypeChunk[rodID].Behaviour;

        if (!(item.Behaviour is FishItem))
        {
            Debug.LogWarning($"{rodID} is not a rod.");
            return;
        }
        
        FishItem fishItem = (FishItem)item.Behaviour;

        if (_playFishingCoroutine != null)
            StopCoroutine(_playFishingCoroutine);

        _playFishingCoroutine = StartCoroutine(PlayFishing(fishItem, rodItem));
    }

    private void Awake()
    {
        _inputs = new PlayerAction();
    }

    private void Start()
    {
        _fishings.Add(_playerIndex, this);
        SetHeight();

        DebugLogConsole.AddCommandInstance("fishing_start", "Start fish game.", nameof(StartFishing), this);
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Fishing.Drag.started += OnDrag;
        _inputs.Fishing.Drag.performed += OnDrag;
        _inputs.Fishing.Drag.canceled += OnDrag;
    }

    private void OnDisable()
    {
        _inputs.Disable();
        _inputs.Fishing.Drag.started -= OnDrag;
        _inputs.Fishing.Drag.performed -= OnDrag;
        _inputs.Fishing.Drag.canceled -= OnDrag;
    }

    private void OnDestroy()
    {
        _fishings.Remove(_playerIndex);
    }

    private void SetHeight()
    {
        _fishArea.sizeDelta = new Vector2(_fishArea.rect.width, _fishHeight);
    }

    private IEnumerator PlayFishing(FishItem fishItem, RodItem rodItem)
    {
        MinMaxFloat catchRange = new MinMaxFloat(0, Mathf.Clamp01(rodItem.CatchSize));

        void SetCatchArea(float point)
        {
            point = Mathf.Clamp01(point);

            float length = catchRange.Length();
            float halfLength = length / 2;
            float offset = 0;

            if (point - halfLength < 0)         // Too low.
                offset = halfLength - point;
            else if (point + halfLength > 1)    // Too high.
                offset = (1 - point) - halfLength;

            catchRange.Min = point - halfLength + offset;
            catchRange.Max = point + halfLength + offset;
        }

        void ConvertToPixels()
        {
            Rect catchArea = _catchArea.rect;
            Rect fishArea = _fishArea.rect;
            _catchArea.offsetMin = new Vector2(catchArea.xMin, fishArea.yMin + _fishHeight  * catchRange.Min);
            _catchArea.offsetMax = new Vector2(catchArea.xMax, fishArea.yMin + _fishHeight * catchRange.Max);
        }

        float timeStart = Time.time;
        float point = 0;

       
        while (true)
        {
            point = Mathf.PingPong(Time.time * 0.1f, 1);
            SetCatchArea(point);
            ConvertToPixels();
            yield return null;
        }     
    }

#if UNITY_EDITOR
    private int _oldHeight;

    private void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    private void _OnValidate()
    {
        if (this == null)
            return;

        if (_oldHeight != _fishHeight)
            SetHeight();

        _oldHeight = _fishHeight;
    }
#endif
}
