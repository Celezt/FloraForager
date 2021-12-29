using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class MessageLog : Singleton<MessageLog>
{
    [SerializeField]
    private int _messageLimit = 6;

    private List<Message> _messages;

    private void Start()
    {
        _messages = new List<Message>();
    }

    private void LateUpdate()
    {

    }

    public void Send(string message, Color color, float time = 1.0f)
    {
        if (_messages.Count >= _messageLimit)
            _messages.RemoveAt(_messages.Count - 1);

        _messages.Add(new Message()
        {
            MessageText = message,
            Color = color,
            Time = time
        });
    }

    private struct Message
    {
        public string MessageText;
        public Color Color;
        public float Time;
    }
}
