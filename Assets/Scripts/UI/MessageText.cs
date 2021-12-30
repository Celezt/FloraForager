using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageText : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _messageText;
    [SerializeField]
    private AnimationCurve _fadeCurve;

    [HideInInspector]
    public bool IsAlive;

    private RectTransform _rectTransform;

    private float _speed;
    private float _time;
    private float _totalTime;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(string message, Color color, float speed, float time)
    {
        _messageText.text = message;
        _messageText.color = color;

        _speed = speed;
        _time = _totalTime = time;

        IsAlive = true;
    }

    private void Update()
    {
        Vector2 position = _rectTransform.anchoredPosition;
        position.y += _speed * Time.deltaTime;
        _rectTransform.anchoredPosition = position;

        _time -= Time.deltaTime;
        _messageText.alpha = _fadeCurve.Evaluate(_time / _totalTime);

        if (_time <= 0.0f)
            IsAlive = false;
    }
}
