using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
    [SerializeField]
    private Music[] _Soundtrack;

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
                    StopCoroutine(_CoroutinePlay);
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

            AsyncOperationHandle asyncLoad = music.Asset.LoadAssetAsync<AudioClip>();

            yield return asyncLoad; // wait until complete

            AudioClip audioClip = asyncLoad.Result as AudioClip;

            _AudioSource.clip = audioClip;
            _AudioSource.outputAudioMixerGroup = _Output;
            _AudioSource.volume = music.Volume;
            _AudioSource.pitch = music.Pitch;
            _AudioSource.Play();

            yield return new WaitUntil(() => !_AudioSource.isPlaying && !_IsPaused); // wait until music has stopped playing

            _AudioSource.clip = null; // set to null and release asset to free up memory
            Addressables.Release(asyncLoad);

            if (!_Loop && !_Shuffle && _CurrentTrack == _Soundtrack.Length - 1)
            {
                _CoroutinePlay = null;
                _IsActive = false;

                yield break;
            }

            _CurrentTrack = _Shuffle ? Random.Range(0, _Soundtrack.Length) : (_CurrentTrack + 1) % _Soundtrack.Length;
        }
    }

    public void SetTrack(int track, bool shuffle = false, bool loop = false)
    {
        _CurrentTrack = track;
        _Shuffle = shuffle;
        _Loop = loop;

        _AudioSource.Stop();
        _CoroutinePlay = null;

        IsActive = true;
    }
    public void SetTrack(string trackName, bool shuffle = false, bool loop = false)
    {
        // don't need dictionary since the array will usually be small
        //
        for (int i = 0; i < _Soundtrack.Length; ++i)
        {
            if (_Soundtrack[i].Name == trackName)
            {
                SetTrack(i, shuffle, loop);
                break;
            }
        }
    }

    [System.Serializable]
    private class Music
    {
        [SerializeField]
        private string _Name;
        [SerializeField]
        private AssetReference _Asset;

        [Space(5)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;

        public string Name => _Name;

        public AssetReference Asset => _Asset;

        public float Volume => _Volume;
        public float Pitch => _Pitch;
    }
}
