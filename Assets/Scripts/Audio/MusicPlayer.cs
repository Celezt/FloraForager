using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Sirenix.OdinInspector;
using MyBox;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField]
    private AudioSource _AudioSource;
    [SerializeField]
    private AudioMixerGroup _Output;
    [SerializeField]
    private bool _Loop;
    [SerializeField]
    private bool _Shuffle;

    [Space(5)]
    [SerializeField, ListDrawerSettings(ShowItemCount = false, Expanded = true)]
    private Music[] _Soundtrack;

    public event System.Action OnComplete = delegate { };

    private bool _IsActive;
    private int _CurrentTrack;
    private bool _IsPaused;

    private Coroutine _CoroutinePlay;

    public bool IsActive
    {
        get => _IsActive; 
        set 
        {
            if (_IsActive = value)
            {
                if (_CoroutinePlay == null)
                    _CoroutinePlay = StartCoroutine(PlayMusic());
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
    public bool Loop
    {
        get => _Loop;
        set => _Loop = value;
    }
    public bool Paused
    {
        get => _IsPaused; 
        set
        {
            if (_IsPaused = value)
                _AudioSource.Pause();
            else 
                _AudioSource.Play();
        }
    }

    private void Start()
    {
        _CoroutinePlay = StartCoroutine(PlayMusic());
    }

    private IEnumerator PlayMusic()
    {
        if (_Soundtrack.Length == 0)
            yield break;

        while (true)
        {
            Music music = _Soundtrack[_CurrentTrack];

            _AudioSource.clip = music.AudioClip;
            _AudioSource.outputAudioMixerGroup = _Output;
            _AudioSource.volume = music.Volume;
            _AudioSource.pitch = music.Pitch;
            _AudioSource.loop = music.Loop;
            _AudioSource.Play();

            yield return new WaitUntil(() => !_AudioSource.isPlaying && !_IsPaused); // wait until music has stopped playing

            _AudioSource.clip = null;

            if (!_Loop && !_Shuffle && _CurrentTrack == _Soundtrack.Length - 1)
            {
                _CoroutinePlay = null;
                _IsActive = false;

                yield break;
            }

            _CurrentTrack = _Shuffle ? Random.Range(0, _Soundtrack.Length) : (_CurrentTrack + 1) % _Soundtrack.Length;

            OnComplete.Invoke();
        }
    }

    public void SetTrack(int track, float fadeTime = 0.0f, bool shuffle = false, bool loop = true)
    {
        if (_CurrentTrack == track)
            return;

        StartCoroutine(FadeOutMusic(track, fadeTime, shuffle, loop));
    }

    private IEnumerator FadeOutMusic(int track, float fadeTime, bool shuffle, bool loop)
    {
        float currentVolume = _AudioSource.volume;
        for (float i = fadeTime; i >= 0; i -= Time.deltaTime)
        {
            _AudioSource.volume = currentVolume * (i / fadeTime);
            yield return null;
        }

        void CompleteAction()
        {
            _CurrentTrack = track;
            _Shuffle = shuffle;
            _Loop = loop;

            OnComplete -= CompleteAction;
        };

        OnComplete += CompleteAction;
        _AudioSource.Stop();
    }

    [System.Serializable]
    private class Music
    {
        [SerializeField]
        private AudioClip _Clip;
        [SerializeField]
        private bool _Loop;

        [Space(5)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;

        public AudioClip AudioClip => _Clip;
        public bool Loop => _Loop;

        public float Volume => _Volume;
        public float Pitch => _Pitch;
    }
}
