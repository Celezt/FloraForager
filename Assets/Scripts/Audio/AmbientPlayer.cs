using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Sirenix.OdinInspector;
using MyBox;

public class AmbientPlayer : Singleton<AmbientPlayer>
{
    [SerializeField]
    private AudioSource _AudioSource;
    [SerializeField]
    private AudioMixerGroup _Output;
    [SerializeField]
    private bool _Loop;
    [SerializeField]
    private bool _Shuffle;

    [SerializeField, ListDrawerSettings(ShowItemCount = false, Expanded = true)]
    private Ambient[] _Soundtrack;

    private bool _IsActive;
    private int _CurrentTrack;

    private Coroutine _CoroutinePlay;

    public bool IsActive
    {
        get => _IsActive;
        set
        {
            if (_IsActive = value)
            {
                if (_CoroutinePlay == null)
                    _CoroutinePlay = StartCoroutine(PlayAmbient());
            }
            else
            {
                if (_CoroutinePlay != null)
                {
                    StopCoroutine(_CoroutinePlay);
                    _CoroutinePlay = null;
                }
            }
        }
    }

    private void Start()
    {
        _CoroutinePlay = StartCoroutine(PlayAmbient());
    }

    private IEnumerator PlayAmbient()
    {
        if (_Soundtrack.Length == 0)
            yield break;

        while (true)
        {
            Ambient ambient = _Soundtrack[_CurrentTrack];

            _AudioSource.clip = ambient.AudioClip;
            _AudioSource.outputAudioMixerGroup = _Output;
            _AudioSource.volume = ambient.Volume;
            _AudioSource.pitch = ambient.Pitch;
            _AudioSource.Play();

            yield return new WaitUntil(() => !_AudioSource.isPlaying); // wait until music has stopped playing

            _AudioSource.clip = null;

            if (!_Loop && !_Shuffle && _CurrentTrack == _Soundtrack.Length - 1)
            {
                _CoroutinePlay = null;
                _IsActive = false;

                yield break;
            }

            _CurrentTrack = _Shuffle ? Random.Range(0, _Soundtrack.Length) : (_CurrentTrack + 1) % _Soundtrack.Length;
        }
    }

    [System.Serializable]
    private class Ambient
    {
        [SerializeField]
        private AudioClip _Clip;

        [Space(5)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;

        public AudioClip AudioClip => _Clip;

        public float Volume => _Volume;
        public float Pitch => _Pitch;
    }
}
