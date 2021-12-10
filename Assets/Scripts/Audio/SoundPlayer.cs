using System.Threading.Tasks;
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
using IngameDebugConsole;
using Celezt.Time;

public class SoundPlayer : Singleton<SoundPlayer>
{
    [SerializeField, Min(1)]
    private int _InitialPoolSize = 5;
    [SerializeField]
    private AudioMixerGroup _Output;

    [Space(10)]
    [SerializeField, ListDrawerSettings(ShowItemCount = false, Expanded = true)]
    private Sound[] _SoundEffects;
    [Space(10)]
    [SerializeField, ListDrawerSettings(ShowItemCount = false, Expanded = true)]
    private Sound[] _DialogueSounds;

    private AudioSource[] _AudioSourcePool;
    private int _CurrentPoolSize;
    private int _PoolIndex;

    private Dictionary<string, Sound> _Sounds;
    private Dictionary<string, Duration> _Cooldowns;

    private int PoolIndex
    {
        get => _PoolIndex;
        set
        {
            if ((_PoolIndex = value) >= _CurrentPoolSize)
            {
                ExpandPool((_CurrentPoolSize * 2) + 1);
            }
            else if ((_PoolIndex = value) < _CurrentPoolSize / 4)
            {
                ShrinkPool(Mathf.Max(_CurrentPoolSize / 2, _InitialPoolSize));
            }
        }
    }

    private IEnumerator Start()
    {
        _SoundEffects = _SoundEffects.Concat(_DialogueSounds).ToArray();

        _AudioSourcePool = new AudioSource[_InitialPoolSize];
        for (int i = 0; i < _AudioSourcePool.Length; ++i)
            _AudioSourcePool[i] = gameObject.AddComponent<AudioSource>();

        _CurrentPoolSize = _InitialPoolSize;
        PoolIndex = 0;

        _Cooldowns = new Dictionary<string, Duration>();
        _Sounds = new Dictionary<string, Sound>();

        AsyncOperationHandle<AudioClip>[] handles = new AsyncOperationHandle<AudioClip>[_SoundEffects.Length];

        for (int i = 0; i < _SoundEffects.Length; ++i)
        {
            Sound sound = _SoundEffects[i];
            (handles[i] = sound.Load()).Completed += (AsyncOperationHandle<AudioClip> handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    sound.AudioClip = handle.Result;
                    sound.Name = handle.Result.name;

                    _Sounds[sound.Name] = sound;
                }
                else
                    Debug.LogError($"something went wrong when trying to load sound [{i}]");
            };
        }

        yield return new WaitUntil(() => handles.All(h =>
            h.Status == AsyncOperationStatus.Succeeded ||
            h.Status == AsyncOperationStatus.Failed));

        _SoundEffects = null;
        _DialogueSounds = null;

        DebugLogConsole.AddCommandInstance("play.sound", "Plays sound", nameof(Play), this);
    }

    public void Play(string name, float volumeChange = 0.0f, float pitchChange = 0.0f, int repeatCount = 0, float cooldown = 0.0f, bool loop = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (!TryGetSound(name, out Sound sound))
            return;

        StartCoroutine(PlaySound(sound, volumeChange, pitchChange, repeatCount, cooldown, loop));
    }

    private IEnumerator PlaySound(Sound sound, float volumeChange = 0.0f, float pitchChange = 0.0f, int repeatCount = 0, float cooldown = 0.0f, bool loop = false)
    {
        int poolIndex = 0;

        while (_AudioSourcePool[poolIndex].isPlaying)
        {
            PoolIndex++; 
            poolIndex++;
        }

        AudioSource source = _AudioSourcePool[poolIndex];
        sound.AudioSource = source;

        for (int i = 0; i < ((repeatCount > 0) ? repeatCount : 1); ++i)
        {
            if (!_Cooldowns.ContainsKey(sound.Name))
                _Cooldowns.Add(sound.Name, Duration.Empty);

            while (_Cooldowns[sound.Name].IsActive)
                yield return null;

            if (cooldown > float.Epsilon)
                _Cooldowns[sound.Name] = new Duration(cooldown);

            SetAudioSource(source, sound);

            source.volume += volumeChange;
            source.pitch += pitchChange;
            source.loop = loop;

            source.Play();

            yield return new WaitUntil(() => !source.isPlaying);
        }

        sound.AudioSource = null;
        source.clip = null;

        while (PoolIndex > 0 && !_AudioSourcePool[PoolIndex].isPlaying)
            --PoolIndex;
    }

    public void Stop(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (!TryGetSound(name, out Sound sound))
            return;

        if (sound.AudioSource != null)
            sound.AudioSource.Stop();
    }

    /// <summary>
    /// add AudioSource to object with sound that can be saved and played whenever
    /// </summary>
    public AudioSource AddAudioSource(GameObject gameObject, string name, float spatialBlend = 1.0f)
    {
        if (!TryGetSound(name, out Sound sound))
            return null;

        // add Audio Source to gameobject to use 3D audio
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        SetAudioSource(audioSource, sound);
        audioSource.spatialBlend = spatialBlend;

        return audioSource;
    }

    /// <summary>
    /// play a sound at point in world
    /// </summary>
    public void PlayAtPoint(string name, Vector3 position, float spatialBlend = 1.0f)
    {
        if (!TryGetSound(name, out Sound sound))
            return;

        // Create new empty object at position
        GameObject soundObject = new GameObject();
        soundObject.transform.position = position;

        // Add audio Source to empty object and play sound on it
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        SetAudioSource(audioSource, sound);
        audioSource.spatialBlend = spatialBlend;

        audioSource.Play();

        // Destroy object after sound has finished playing
        Destroy(soundObject, audioSource.clip.length);
    }

    public void SetSound(string name, float volume, float pitch = 1.0f, float spatialBlend = 0, float cooldown = 0, int repeatCount = 0)
    {
        if (!TryGetSound(name, out Sound sound))
            return;

        sound.Set(volume, pitch, spatialBlend);
    }
    public bool TryGetSound(string name, out Sound sound)
    {
        return _Sounds.TryGetValue(name, out sound);
    }

    private void SetAudioSource(AudioSource audioSource, Sound sound)
    {
        if (audioSource == null)
            return;

        audioSource.clip = sound.AudioClip;
        audioSource.outputAudioMixerGroup = _Output;

        audioSource.volume = sound.Volume;
        audioSource.pitch = sound.Pitch;
        audioSource.spatialBlend = sound.SpatialBlend;
    }

    private void ExpandPool(int size)
    {
        if (size <= _CurrentPoolSize)
            return;

        AudioSource[] newPool = new AudioSource[size];
        for (int i = 0; i < _CurrentPoolSize; ++i)
            newPool[i] = _AudioSourcePool[i];
        for (int i = _CurrentPoolSize; i < size; ++i)
            newPool[i] = gameObject.AddComponent<AudioSource>();

        _CurrentPoolSize = (_AudioSourcePool = newPool).Length;
    }
    private void ShrinkPool(int size)
    {
        if (size >= _CurrentPoolSize)
            return;

        for (int i = size; i < _CurrentPoolSize; ++i)
        {
            AudioSource source = _AudioSourcePool[i];

            if (source == null || source.isPlaying)
                continue;

            Destroy(source);
            _AudioSourcePool[i] = null;
        }

        List<AudioSource> newPool = new List<AudioSource>();
        for (int i = 0; i < _AudioSourcePool.Length; ++i)
        {
            if (_AudioSourcePool[i] != null)
                newPool.Add(_AudioSourcePool[i]);
        }

        _AudioSourcePool = newPool.ToArray();
        _CurrentPoolSize = _AudioSourcePool.Length;
    }

    [System.Serializable]
    public class Sound
    {
        [SerializeField, AssetsOnly]
        private AssetReference _Asset;

        [Space(2)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;
        [SerializeField, HideInInspector, Range(0.0f, 1.0f)]
        private float _SpatialBlend = 0.0f;

        [HideInInspector]
        public string Name;
        [HideInInspector]
        public AudioClip AudioClip;
        [HideInInspector]
        public AudioSource AudioSource; // audio source playing this sound

        public float Volume => _Volume;
        public float Pitch => _Pitch;
        public float SpatialBlend => _SpatialBlend;

        public void Set(float volume, float pitch, float spatialBlend = 0)
        {
            _Volume = volume;
            _Pitch = pitch;
            _SpatialBlend = spatialBlend;
        }

        public AsyncOperationHandle<AudioClip> Load() => _Asset.LoadAssetAsync<AudioClip>();
    }
}
