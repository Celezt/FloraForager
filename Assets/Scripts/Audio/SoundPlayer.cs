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

public class SoundPlayer : Singleton<SoundPlayer>
{
    [SerializeField, Min(1)]
    private int _InitialPoolSize = 5;
    [SerializeField]
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

    private void Start()
    {
        _AudioSourcePool = new AudioSource[_InitialPoolSize];
        for (int i = 0; i < _AudioSourcePool.Length; ++i)
            _AudioSourcePool[i] = gameObject.AddComponent<AudioSource>();

        _CurrentPoolSize = _InitialPoolSize;
        PoolIndex = 0;

        _SoundEffects.ForEach(s => s.Load());

        _Sounds = _SoundEffects.ToDictionary(key => key.Name, value => value);
        _SoundEffects = null;

        _Cooldowns = new Dictionary<string, float>();
    }

    private void Update()
    {
        if (_Cooldowns.Count > 0)
        {
            for (int i = _Cooldowns.Count - 1; i >= 0; --i)
            {
                string key = _Cooldowns.ElementAt(i).Key;
                if ((_Cooldowns[key] -= Time.deltaTime) <= 0.0f)
                {
                    _Cooldowns.Remove(key);
                }
            }
        }
    }

    public void Play(string soundName)
    {
        StartCoroutine(PlaySound(soundName));
    }

    private IEnumerator PlaySound(string name)
    {
        AudioSource source = _AudioSourcePool[PoolIndex++];
        Sound sound = GetSound(name);

        sound.AudioSource = source;

        for (int i = 0; i < ((sound.RepeatCount > 0) ? sound.RepeatCount : 1); ++i)
        {
            while (_Cooldowns.ContainsKey(name))
                yield return null;

            SetAudioSource(source, sound);
            source.Play();

            if (sound.Cooldown > float.Epsilon)
                _Cooldowns.Add(sound.Name, sound.Cooldown);

            yield return new WaitUntil(() => !source.isPlaying);
        }

        sound.AudioSource = null;
        source.clip = null;

        --PoolIndex;
    }

    /// <summary>
    /// add AudioSource to object with sound that can be saved and played whenever
    /// </summary>
    public AudioSource AddAudioSource(GameObject gameObject, string name, float spatialBlend = 1.0f)
    {
        // add Audio Source to gameobject to use 3D audio
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        SetAudioSource(audioSource, GetSound(name));
        audioSource.spatialBlend = spatialBlend;

        return audioSource;
    }

    /// <summary>
    /// play a sound at point in world
    /// </summary>
    public void PlayAtPoint(string name, Vector3 position, float spatialBlend = 1.0f)
    {
        Sound sound = GetSound(name);

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
        Sound sound = GetSound(name);

        if (sound.Name != "error")
            sound.Set(volume, pitch, spatialBlend, cooldown, repeatCount);
    }
    public Sound GetSound(string name)
    {
        if (!_Sounds.ContainsKey(name))
        {
            Debug.LogWarning($"'{name}' does not exist");
            return _Sounds["error"];
        }

        return _Sounds[name];
    }

    private void SetAudioSource(AudioSource audioSource, Sound sound)
    {
        if (audioSource == null)
            return;

        audioSource.clip = sound.AudioClip;
        audioSource.outputAudioMixerGroup = sound.Output;

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
        [SerializeField]
        private string _Name;

        [Space(5)]
        [SerializeField, AssetsOnly]
        private AssetReference _Asset;
        [SerializeField]
        private AudioMixerGroup _Output;

        [Space(5)]
        [SerializeField, Range(0.0f, 1.0f)]
        private float _Volume = 1.0f;
        [SerializeField, Range(0.1f, 3.0f)]
        private float _Pitch = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)]
        private float _SpatialBlend;
        [SerializeField]
        private float _Cooldown = 0.0f;
        [SerializeField]
        private int _RepeatCount;

        private AudioClip _AudioClip;

        [HideInInspector]
        public AudioSource AudioSource; // audio source playing this sound

        public string Name => _Name;

        public AudioClip AudioClip => _AudioClip;
        public AudioMixerGroup Output => _Output;

        public float Volume => _Volume;
        public float Pitch => _Pitch;
        public float SpatialBlend => _SpatialBlend;
        public float Cooldown => _Cooldown;
        public int RepeatCount => _RepeatCount;

        public void Set(float volume, float pitch = 1.0f, float spatialBlend = 0, float cooldown = 0, int repeatCount = 0)
        {
            _Volume = volume;
            _Pitch = pitch;
            _SpatialBlend = spatialBlend;
            _Cooldown = cooldown;
            _RepeatCount = repeatCount;
        }

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
