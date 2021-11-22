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

public class SoundPlayer : Singleton<SoundPlayer>
{
    [SerializeField, Min(1)]
    private int _InitialPoolSize = 5;
    [SerializeField]
    private AudioMixerGroup _Output;

    [Space(10)]
    [SerializeField, ListDrawerSettings(ShowItemCount = false, Expanded = true)]
    private Sound[] _SoundEffects;

    private AudioSource[] _AudioSourcePool;
    private int _CurrentPoolSize;
    private int _PoolIndex;

    private Dictionary<string, Sound> _Sounds;
    private Dictionary<string, float> _Cooldowns;

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
                if (_CurrentPoolSize > 2)
                    ShrinkPool(_CurrentPoolSize / 2);
            }
        }
    }

    private IEnumerator Start()
    {
        _AudioSourcePool = new AudioSource[_InitialPoolSize];
        for (int i = 0; i < _AudioSourcePool.Length; ++i)
            _AudioSourcePool[i] = gameObject.AddComponent<AudioSource>();

        _CurrentPoolSize = _InitialPoolSize;
        PoolIndex = 0;

        _Cooldowns = new Dictionary<string, float>();
        _Sounds = new Dictionary<string, Sound>();

        AsyncOperationHandle<AudioClip>[] handles = new AsyncOperationHandle<AudioClip>[_SoundEffects.Length];

        for (int i = 0; i < _SoundEffects.Length; ++i)
            handles[i] = _SoundEffects[i].Load();

        yield return new WaitUntil(() => handles.All(h =>
            h.Status == AsyncOperationStatus.Succeeded ||
            h.Status == AsyncOperationStatus.Failed));

        for (int i = 0; i < _SoundEffects.Length; ++i)
        {
            if (handles[i].Status == AsyncOperationStatus.Succeeded)
                _Sounds[_SoundEffects[i].Name] = _SoundEffects[i];

            Addressables.Release(handles[i]);
        }

        _SoundEffects = null;

        DebugLogConsole.AddCommandInstance("play.sound", "Plays sound", nameof(Play), this);
    }

    private void Update()
    {
        if (_Cooldowns.Count > 0)
        {
            foreach (string key in _Cooldowns.Keys.ToList())
            {
                if ((_Cooldowns[key] -= Time.deltaTime) <= 0.0f)
                {
                    _Cooldowns.Remove(key);
                }
            }
        }
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
        AudioSource source = _AudioSourcePool[PoolIndex++];
        sound.AudioSource = source;

        for (int i = 0; i < ((repeatCount > 0) ? repeatCount : 1); ++i)
        {
            while (_Cooldowns.ContainsKey(sound.Name))
                yield return null;

            if (cooldown > float.Epsilon)
                _Cooldowns.Add(sound.Name, cooldown);

            SetAudioSource(source, sound);

            source.volume += volumeChange;
            source.pitch += pitchChange;
            source.loop = loop;

            source.Play();

            yield return new WaitUntil(() => !source.isPlaying);
        }

        sound.AudioSource = null;
        source.clip = null;

        --PoolIndex;
    }

    public void Stop(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (!TryGetSound(name, out Sound sound))
        {
            Debug.LogError($"{name} does not exist");
            return;
        }

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

        if (sound != null)
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
        if (size == _CurrentPoolSize || size < _CurrentPoolSize)
            return;

        for (int i = size - 1; i >= _CurrentPoolSize; --i)
        {
            gameObject.AddComponent<AudioSource>();
        }

        _AudioSourcePool = GetComponents<AudioSource>();
        _CurrentPoolSize = _AudioSourcePool.Length;
    }
    private void ShrinkPool(int size)
    {
        if (size == _CurrentPoolSize || size > _CurrentPoolSize)
            return;

        for (int i = _CurrentPoolSize - 1; i >= size; --i)
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
        [SerializeField, HideInInspector]
        private string _Name;
        [SerializeField, AssetsOnly]
        private AssetReference _Asset;

        [Space(2)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;
        [SerializeField, HideInInspector, Range(0.0f, 1.0f)]
        private float _SpatialBlend = 0.0f;

        private AudioClip _AudioClip;

        [HideInInspector]
        public AudioSource AudioSource; // audio source playing this sound

        public string Name => _Name;

        public AssetReference Asset => _Asset;
        public AudioClip AudioClip => _AudioClip;

        public float Volume => _Volume;
        public float Pitch => _Pitch;
        public float SpatialBlend => _SpatialBlend;

        public void Set(float volume, float pitch, float spatialBlend = 0)
        {
            _Volume = volume;
            _Pitch = pitch;
            _SpatialBlend = spatialBlend;
        }

        public AsyncOperationHandle<AudioClip> Load()
        {
            AsyncOperationHandle<AudioClip> handle = _Asset.LoadAssetAsync<AudioClip>();

            handle.Completed += (AsyncOperationHandle<AudioClip> hndl) => 
            {
                if (hndl.Result == null)
                {
                    Debug.LogError("sound was unable to load");
                    return;
                }

                _AudioClip = hndl.Result;
                _Name = hndl.Result.name;
            };

            return handle;
        }
    }
}
