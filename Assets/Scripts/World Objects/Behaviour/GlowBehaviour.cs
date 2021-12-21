using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GlowBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem _GlowEffect;
    [SerializeField] private float _Distance = 10.0f;

    private GameObject _Player;

    private void Start()
    {
        _Player = PlayerInput.GetPlayerByIndex(0).gameObject;
    }

    private void Update()
    {
        float distance = Mathf.Sqrt(
            Mathf.Pow(gameObject.transform.position.x - _Player.transform.position.x, 2) + 
            Mathf.Pow(gameObject.transform.position.z - _Player.transform.position.z, 2));

        if (distance < _Distance)
        {
            if (!_GlowEffect.isPlaying)
                _GlowEffect.Play();
        }
        else
        {
            if (_GlowEffect.isPlaying)
                _GlowEffect.Stop();
        }
    }
}
