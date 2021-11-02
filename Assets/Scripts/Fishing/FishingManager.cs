using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IngameDebugConsole;
using MyBox;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Celezt.Time;

public class FishingManager : MonoBehaviour
{
    private static Dictionary<int, FishingManager> _fishings = new Dictionary<int, FishingManager>();

    public event System.Action OnPlayCallback = delegate { };
    public event System.Action OnCatchCallback = delegate { };
    public event System.Action OnFleeCallback = delegate { };

    [SerializeField] private RectTransform _fishAreaTransform;
    [SerializeField] private RectTransform _catchAreaTransform;
    [SerializeField] private RectTransform _catchPointTransform;
    [SerializeField] private RectTransform _fishPointTransform;
    [SerializeField] private RectTransform _toggleTransform;
    [SerializeField] private Slider _progressBar;
#if UNITY_EDITOR
    [OnValueChanged(nameof(OnHeightChange))]
#endif
    [SerializeField, Min(0)] private int _barHeight = 500;
    [SerializeField, Min(0)] private int _playerIndex;
    [SerializeField, Min(0)] private float _gravity = 9.82f;
    [SerializeField, Min(0)] private float _progressSpeed = 5f;
    [SerializeField, Min(0)] private float _startInvisibilityFrame = 4;

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

        ItemType fishType = ItemTypeSettings.Instance.ItemTypeChunk[fishID];

        if (!(fishType.Behaviour is FishItem))
        {
            Debug.LogWarning($"{fishID} is not a fish.");
            return;
        }

        if (!ItemTypeSettings.Instance.ItemTypeChunk.ContainsKey(rodID))
        {
            Debug.LogWarning($"{rodID} does not exist.");
            return;
        }

        ItemType rodType = ItemTypeSettings.Instance.ItemTypeChunk[rodID];

        if (!(rodType.Behaviour is RodItem))
        {
            Debug.LogWarning($"{rodID} is not a rod.");
            return;
        }
        
        FishItem fishItem = (FishItem)fishType.Behaviour;
        RodItem rodItem = (RodItem)rodType.Behaviour;

        _fishPointTransform.GetComponent<Image>().sprite = ItemTypeSettings.Instance.ItemIconChunk[fishID];

        _toggleTransform.gameObject.SetActive(true);

        if (_playFishingCoroutine != null)
            StopCoroutine(_playFishingCoroutine);

        _playFishingCoroutine = StartCoroutine(PlayFishing(fishItem, rodItem));
    }

    public void StopFishing()
    {
        if (_playFishingCoroutine != null)
            StopCoroutine(_playFishingCoroutine);

        _toggleTransform.gameObject.SetActive(false);
    }

    private void Awake()
    {
        _inputs = new PlayerAction();

        OnFleeCallback += () => 
        {
            StopFishing();
        };
        OnCatchCallback += () =>
        {
            StopFishing();
        };
    }

    private void Start()
    {
        _fishings.Add(_playerIndex, this);
        SetHeight();

        _toggleTransform.gameObject.SetActive(false);

        DebugLogConsole.AddCommandInstance("fishing.start", "Start fish game.", nameof(StartFishing), this);
        DebugLogConsole.AddCommandInstance("fishing.stop", "Stop fish game.", nameof(StopFishing), this);
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
        _fishAreaTransform.sizeDelta = new Vector2(_fishAreaTransform.rect.width, _barHeight);
        ((RectTransform)_progressBar.transform).sizeDelta = new Vector2(((RectTransform)_progressBar.transform).rect.width, _barHeight);
    }

    private IEnumerator PlayFishing(FishItem fishItem, RodItem rodItem)
    {
        OnPlayCallback.Invoke();

        MinMaxFloat catchRange = new MinMaxFloat(0, Mathf.Clamp01(rodItem.CatchSize));

        float timer = 0;
        float dragForce = rodItem.DragForce;
        float dragDamp = rodItem.DragDamp;
        float dragWeight = rodItem.DragWeight;
        float bounciness = rodItem.Bounciness;
        float gravityForce = _gravity * dragWeight;
        float rodStar = (int)((IStar)rodItem).Star;
        float catchForce = 0;
        float catchVelocity = 0;
        float catchPoint = 0;

        float fishLastChangeTime = 0;
        float fishPoint = 0;
        float fishPointSnapshot = 0;
        float fishPointPostProcessed = 0;
        AnimationCurve[] fishUpPatterns = fishItem.UpPatterns;
        AnimationCurve[] fishIdlePatterns = fishItem.IdlePatterns;
        AnimationCurve[] fishDownPatterns = fishItem.DownPatterns;
        AnimationCurve currentPattern = fishUpPatterns[Random.Range(0, fishUpPatterns.Length - 1)];   // Always start swimming up.
        int fishStar = (int)((IStar)fishItem).Star;
        float fishHaste = fishItem.Haste;
        float fishCalmness = fishItem.Calmness;
        float fishRandomness = fishItem.Randomness;
        
        float progressValue = 0;
        float deltaTime = 0;
        float randomValue = Random.value;
        Duration invisibilityFrame = new Duration(_startInvisibilityFrame);   // Invisibility frame at the beginning of the game.

        void CatchPhysics()
        {
            catchForce = _drag > 0.5f ? Mathf.Lerp(catchForce, dragForce, dragDamp * deltaTime) : 0;    // Player input.
            float acceleration = (-gravityForce + catchForce) / dragWeight;
            catchVelocity += acceleration * deltaTime;
            catchPoint += 0.1f * (catchVelocity * deltaTime + (acceleration * deltaTime * deltaTime) / 2);
        }

        void SetBouncing()
        {
            if (catchPoint > 1)
                catchVelocity = 0;
            else if (catchPoint < 0)
                catchVelocity *= -bounciness;
        }

        void SetCatchArea()
        {
            catchPoint = Mathf.Clamp01(catchPoint);

            float length = catchRange.Length();
            float halfLength = length / 2;
            float offset = 0;

            if (catchPoint - halfLength < 0)         // Too low.
                offset = halfLength - catchPoint;
            else if (catchPoint + halfLength > 1)    // Too high.
                offset = (1 - catchPoint) - halfLength;

            catchRange.Min = catchPoint - halfLength + offset;
            catchRange.Max = catchPoint + halfLength + offset;
        }

        void SetProgressBar()
        {
            MinMaxFloat roundedCatchRange = new MinMaxFloat(Mathf.Round(catchRange.Min * 100.0f) / 100.0f, 
                                                            Mathf.Round(catchRange.Max * 100.0f) / 100.0f);
            if (roundedCatchRange.IsInRange(fishPointPostProcessed))
                progressValue += (rodStar >= fishStar ? ((fishStar + rodStar / 5) / fishStar) * 2f : ((rodStar + fishStar / 5) / fishStar) * 2f) * (_progressSpeed / 100) * deltaTime;
            else
                progressValue -= (rodStar >= fishStar ? (rodStar + fishStar / 5) / rodStar  : (fishStar + rodStar / 5) / rodStar) * (_progressSpeed / 100) * deltaTime;

            progressValue = Mathf.Clamp01(progressValue);

            _progressBar.value = progressValue;
        }

        void FishBehaviour()
        {
            if (timer > fishLastChangeTime + randomValue * 2 + fishCalmness * 2)
            {
                fishLastChangeTime = timer;
                fishPointSnapshot = fishPoint;
                randomValue = Random.value;

                currentPattern = new System.Func<AnimationCurve>(() =>
                {
                    if (randomValue > 0.5f + fishCalmness / 2)      // Up
                        return fishUpPatterns[Random.Range(0, fishUpPatterns.Length - 1)];
                    else if (randomValue < 0.5f - fishCalmness / 2) // Down
                        return fishDownPatterns[Random.Range(0, fishDownPatterns.Length - 1)];
                    else                                            // Idle
                        return fishIdlePatterns[Random.Range(0, fishIdlePatterns.Length - 1)];
                })();

                currentPattern.keys[0].value = 0;   // Fist key will always start at zero.
            }

            float noise = fishRandomness * (0.5f - Mathf.PerlinNoise(timer * fishHaste / 2, 0)) / 10;
            fishPoint = Mathf.Clamp01(fishPointSnapshot + currentPattern.Evaluate(fishHaste * (timer - fishLastChangeTime) / 10));
            fishPointPostProcessed = Mathf.Clamp01(fishPoint + noise);
        }

        void ConvertToPixels()
        {
            Rect catchArea = _catchAreaTransform.rect;
            Rect fishArea = _fishAreaTransform.rect;
            _catchAreaTransform.offsetMin = new Vector2(catchArea.xMin, fishArea.yMin + _barHeight * catchRange.Min);
            _catchAreaTransform.offsetMax = new Vector2(catchArea.xMax, fishArea.yMin + _barHeight * catchRange.Max);

            _catchPointTransform.SetPositionY(fishArea.yMin + _barHeight * catchPoint);
            _fishPointTransform.SetPositionY(fishArea.yMin + _barHeight * fishPointPostProcessed);
        }

        while (true)
        {
            deltaTime = Time.deltaTime;
            timer += deltaTime;

            if (progressValue <= 0)
            {
                if (!invisibilityFrame.IsActive)    // Lost.
                {
                    OnFleeCallback.Invoke();
                    yield break;
                }
            }
            else if (progressValue >= 1)            // Won.
            {
                OnCatchCallback.Invoke();
                yield break;
            }

            CatchPhysics();
            SetBouncing();
            SetCatchArea();
            SetProgressBar();
            FishBehaviour();
            ConvertToPixels();

            yield return null;
        }     
    }

#if UNITY_EDITOR
    private void OnHeightChange() => SetHeight();
#endif
}
