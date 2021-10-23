using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using MyBox;

public class AmbientPlayer : Singleton<AmbientPlayer>
{
    [SerializeField]
    private AudioSource _AudioSource;
    [SerializeField]
    private bool _Loop;
    [SerializeField]
    private bool _Shuffle;

    [SerializeField]
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
                    StopCoroutine(_CoroutinePlay);
            }
        }
    }

    private void Start()
    {
        _Soundtrack.ForEach(a => a.Load());
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
            _AudioSource.outputAudioMixerGroup = ambient.Output;
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
    public class Ambient
    {
        [SerializeField]
        private string _Name;

        [Space(5)]
        [SerializeField]
        private AssetReference _Asset;
        [SerializeField]
        private AudioMixerGroup _Output;

        [Space(5)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;

        private AudioClip _AudioClip;

        public string Name => _Name;

        public AudioClip AudioClip => _AudioClip;
        public AudioMixerGroup Output => _Output;

        public float Volume => _Volume;
        public float Pitch => _Pitch;

        public void Load()
        {
            Addressables.LoadAssetAsync<AudioClip>(_Asset).Completed += (AsyncOperationHandle<AudioClip> handle) =>
            {
                _AudioClip = handle.Result;
                Addressables.Release(handle);
            };
        }
    }
}
