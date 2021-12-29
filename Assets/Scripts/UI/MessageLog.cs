using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MyBox;

public class MessageLog : Singleton<MessageLog>
{
    [SerializeField]
    private GameObject _messagePrefab;
    [SerializeField]
    private int _messageLimit = 6;
    [SerializeField]
    private float _offset = 0.35f;

    private List<MessageText> _messages;

    private GameObject _player;
    private Bounds _playerBounds;

    private Canvas _Canvas;
    private RectTransform _CanvasRect;

    private void Awake()
    {
        _messages = new List<MessageText>();

        _Canvas = GetComponentInParent<Canvas>().rootCanvas;
        _CanvasRect = _Canvas.GetComponent<RectTransform>();
    }

    private void Start()
    {
        _player = PlayerInput.GetPlayerByIndex(0).gameObject;
        _playerBounds = _player.GetComponent<Collider>().bounds;
    }

    private void LateUpdate()
    {
        for (int i = _messages.Count - 1; i >= 0; --i)
        {
            MessageText messageText = _messages[i];
            if (!messageText.IsAlive)
            {
                Destroy(messageText.gameObject);
                _messages.RemoveAt(i);
            }
        }

        if (_player == null)
            return;

        transform.position = CanvasUtility.WorldToCanvasPosition(_Canvas, _CanvasRect, Camera.main,
            _player.transform.position + Vector3.up * (_playerBounds.size.y + _offset));
    }

    public void Send(string message, Color color, float speed = 12.0f, float time = 5.0f)
    {
        if (_messages.Count >= _messageLimit && _messages.Count > 0)
        {
            Destroy(_messages.First().gameObject);
            _messages.RemoveAt(0);
        }

        MessageText messageText = Instantiate(_messagePrefab, transform).GetComponent<MessageText>();

        messageText.Initialize(message, color, speed, time);

        _messages.Add(messageText);
    }
}
