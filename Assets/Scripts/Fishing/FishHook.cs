using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using MyBox;
using TMPro;

public class FishHook : Singleton<FishHook>
{
    [SerializeField]
    private GameObject _HookGame;
    [SerializeField]
    private Slider _FishSlider;
    [SerializeField]
    private GameObject _FishPoint;
    [SerializeField]
    private RectTransform _HookArea;

    public event System.Action<string> OnHook = delegate { };
    public event System.Action OnPlay = delegate { };

    private PlayerAction _PlayerAction;

    private RectTransform _Rect;

    private float _HookPosition;
    private float _HookRange;

    private float _FishPosition;
    private float _FishSpeed;
    private float _FishSpawnTime;
    private float _FishLifetime;

    private bool _FishNear = false;

    private bool _FishHooked = false;
    private bool _Canceled = false;

    private float LeftSide => _HookPosition - (_HookRange / 2.0f);
    private float RightSide => _HookPosition + (_HookRange / 2.0f);

    private void Awake()
    {
        _PlayerAction = new PlayerAction();

        _Rect = _HookGame.GetComponent<RectTransform>();

        _HookGame.SetActive(false);
    }

    private void OnEnable()
    {
        _PlayerAction.Enable();
        _PlayerAction.Fishing.Hook.started += OnTryHook;
        _PlayerAction.Fishing.Cancel.started += OnCancel;
    }

    private void OnDisable()
    {
        _PlayerAction.Disable();
        _PlayerAction.Fishing.Hook.started -= OnTryHook;
        _PlayerAction.Fishing.Cancel.started -= OnCancel;
    }

    public IEnumerator PlayHook(RodItem rod, FishBaitItem fishBait)
    {
        OnPlay.Invoke();

        _FishPosition = Random.Range(0f, 1f);
        _FishSpeed = Random.Range(0.3f, 0.35f);
        _FishSpawnTime = Random.Range(2f, 3f);
        _FishLifetime = Random.Range(5f, 7f);

        _FishNear = false;
        _FishHooked = false;
        _Canceled = false;

        _FishSlider.value = _FishPosition;
        _FishPoint.SetActive(_FishNear);

        _HookPosition = 0.5f;
        _HookArea.sizeDelta = new Vector2(rod.CatchSize * _Rect.rect.width * 0.35f, _HookArea.sizeDelta.y);
        _HookRange = _HookArea.rect.width / _Rect.rect.width; // x / width = catchSize

        _HookGame.SetActive(true);

        int direction = Random.Range(0f, 1f) > 0.5f ? 1 : -1;

        float fishSpawnTime = _FishSpawnTime;
        float fishLifetime = _FishLifetime;

        void FishMovement()
        {
            float oldPosition = _FishPosition;
            _FishPosition += _FishSpeed * direction * Time.deltaTime;

            if (_FishPosition <= 0f || _FishPosition >= 1f)
            {
                direction *= -1;
                _FishPosition = oldPosition;
            }

            _FishSlider.value = _FishPosition;
        }
        void FishLifetime()
        {
            if (!_FishNear)
                return;

            fishLifetime -= Time.deltaTime;
            if (fishLifetime <= 0f)
            {
                _FishNear = false;
                fishSpawnTime = _FishSpawnTime;
            }
        }
        void FishSpawn()
        {
            if (_FishNear)
                return;

            fishSpawnTime -= Time.deltaTime;
            if (fishSpawnTime <= 0f)
            {
                _FishNear = true;
                fishLifetime = _FishLifetime;

                direction = Random.Range(0f, 1f) > 0.5f ? 1 : -1;
                _FishPosition = Random.Range(0f, 1f);

                _FishSlider.value = _FishPosition;
            }
        }
        void FishHooked()
        {
            string fishID = FishPool.Instance.GetFish(fishBait);
            OnHook.Invoke(fishID);

            _HookGame.SetActive(false);
        }
        void Canceled()
        {
            OnHook.Invoke(string.Empty);

            _HookGame.SetActive(false);
        }

        while (true)
        {
            if (_FishHooked)
            {
                FishHooked();
                yield break;
            }
            if (_Canceled)
            {
                Canceled();
                yield break;
            }

            FishMovement();
            FishLifetime();
            FishSpawn();

            _FishPoint.SetActive(_FishNear);

            yield return null;
        }
    }

    public void OnTryHook(InputAction.CallbackContext context)
    {
        if (DebugManager.IsFocused || !_HookGame.activeSelf || !_FishNear)
            return;

        if (_FishPosition >= LeftSide && _FishPosition <= RightSide)
            _FishHooked = true;
        else
            OnCancel(context);
    }
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (DebugManager.IsFocused || !_HookGame.activeSelf)
            return;

        _Canceled = true;
    }
}