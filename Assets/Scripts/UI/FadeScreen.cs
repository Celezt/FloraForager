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
    private Coroutine _FadeCoroutine;

    public bool IsActive => _FadeCoroutine != null;
    public float Value => _CanvasGroup.alpha;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartFadeIn(float fadeTime)
    {
        if (_FadeCoroutine != null)
            StopCoroutine(_FadeCoroutine);
        
        _FadeCoroutine = StartCoroutine(FadeIn(fadeTime));
    }
    public void StartFadeOut(float fadeTime)
    {
        if (_FadeCoroutine != null)
            StopCoroutine(_FadeCoroutine);

        _FadeCoroutine = StartCoroutine(FadeOut(fadeTime));
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

        _FadeCoroutine = null;
        OnEndFade.Invoke();
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

        _FadeCoroutine = null;
        OnEndFade.Invoke();
    }

    public void SetFade(float fade)
    {
        _CanvasGroup.alpha = fade;
    }
}
