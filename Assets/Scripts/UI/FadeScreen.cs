using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public class FadeScreen : Singleton<FadeScreen>
{
    [HideInInspector]
    public event System.Action OnStartFade = delegate { };
    [HideInInspector]
    public event System.Action OnEndFade = delegate { };

    private CanvasGroup _CanvasGroup;

    private bool _CoroutineIsRunning;

    public bool IsActive => _CoroutineIsRunning;
    public float Value => _CanvasGroup.alpha;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartFadeIn(float fadeTime)
    {
        if (!_CoroutineIsRunning)
        {
            StartCoroutine(FadeIn(fadeTime));
            _CoroutineIsRunning = true;
        }
    }
    public void StartFadeOut(float fadeTime)
    {
        if (!_CoroutineIsRunning)
        {
            StartCoroutine(FadeOut(fadeTime));
            _CoroutineIsRunning = true;
        }
    }

    private IEnumerator FadeIn(float fadeTime)
    {
        OnStartFade.Invoke();

        _CanvasGroup.alpha = 0.0f;

        for (float i = 0; i <= fadeTime; i += Time.deltaTime)
        {
            _CanvasGroup.alpha = i / fadeTime;
            yield return null;
        }

        _CanvasGroup.alpha = 1.0f;
        _CanvasGroup.blocksRaycasts = true;

        OnEndFade.Invoke();
        _CoroutineIsRunning = false;
    }
    private IEnumerator FadeOut(float fadeTime)
    {
        OnStartFade.Invoke();

        _CanvasGroup.alpha = 1.0f;
        _CanvasGroup.blocksRaycasts = false;

        for (float i = fadeTime; i >= 0; i -= Time.deltaTime)
        {
            _CanvasGroup.alpha = i / fadeTime;
            yield return null;
        }

        _CanvasGroup.alpha = 0.0f;

        OnEndFade.Invoke();
        _CoroutineIsRunning = false;
    }

    public void SetFade(float fade)
    {
        _CanvasGroup.alpha = fade;
    }
}
