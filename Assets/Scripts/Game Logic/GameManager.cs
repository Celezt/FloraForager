using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

[CreateAssetMenu(fileName = "GameManager", menuName = "Game Logic/Game Manager")]
[System.Serializable]
public class GameManager : SerializedScriptableSingleton<GameManager>
{
    public StreamScriptableObject Stream => _stream;

    [NonSerialized]
    private StreamScriptableObject _stream = new StreamScriptableObject();

    public class StreamScriptableObject
    {
        /// <summary>
        /// Temporary stream capacity. The oldest non dirty object gets released if reached capacity.
        /// </summary>
        public int StreamCapacity
        {
            get => _streamCapacity;
            set
            {
                _streamCapacity = value;

                while (_streamCapacity < _streamQueue.Count)
                {
                    Hash128 hash = _streamQueue.Dequeue();
                    _streamedObjects.Remove(hash);
                }
            }
        }

        private HashSet<Hash128> _dirtyObjects = new HashSet<Hash128>();
        private Dictionary<Hash128, ScriptableObject> _streamedObjects = new Dictionary<Hash128, ScriptableObject>();
        private Queue<Hash128> _streamQueue = new Queue<Hash128>();

        private int _streamCapacity = 64;
        private int _streamOffset;

        public WeakReference<T>? LoadTemp<T>(string id) where T : ScriptableObject => LoadTemp<T>(Hash128.Compute(id));
        public WeakReference<T>? LoadTemp<T>(Hash128 hash) where T : ScriptableObject
        {
            if (_streamedObjects.ContainsKey(hash))
                return null;

            T obj = ScriptableObject.CreateInstance<T>();

            _streamedObjects.Add(hash, obj);

            _streamQueue.Enqueue(hash);
            if (_streamCapacity + _streamOffset < _streamQueue.Count)
                while (_streamQueue.Count > 0)
                {
                    Hash128 outHash = _streamQueue.Dequeue();

                    if (_streamedObjects.ContainsKey(outHash))
                    {
                        if (!_dirtyObjects.Contains(outHash))
                        {
                            Release(outHash);
                            break;
                        }

                        _streamOffset--;
                    }
                }

            return new WeakReference<T>(obj);
        }

        public T? LoadPersistent<T>(string id) where T : ScriptableObject => LoadPersistent<T>(Hash128.Compute(id));
        public T? LoadPersistent<T>(Hash128 hash) where T : ScriptableObject
        {
            if (_streamedObjects.ContainsKey(hash))
                return null;

            T obj = ScriptableObject.CreateInstance<T>();

            _streamedObjects.Add(hash, obj);

            return obj;
        }

        public bool Release(string id) => Release(Hash128.Compute(id));
        public bool Release(Hash128 hash)
        {
            if (!_streamedObjects.TryGetValue(hash, out ScriptableObject obj))
                return false;

            if (_streamQueue.Contains(hash))
                _streamOffset++;

            Destroy(obj);

            _streamedObjects.Remove(hash);
            _dirtyObjects.Remove(hash);

            return true;
        }

        public WeakReference<T?> Get<T>(string id) where T : ScriptableObject => Get<T>(Hash128.Compute(id));
        public WeakReference<T?> Get<T>(Hash128 hash) where T : ScriptableObject
        {
            ScriptableObject obj = _streamedObjects[hash];

            return new WeakReference<T?>((T)obj);  
        }

        /// <summary>
        /// Set dirty to save the asset on next saving.
        /// </summary>
        /// <param name="hash"></param>
        public void SetDirty(Hash128 hash)
        {
            _dirtyObjects.Add(hash);

            if (_streamQueue.Contains(hash))
                _streamOffset++;
        }
    }
}
