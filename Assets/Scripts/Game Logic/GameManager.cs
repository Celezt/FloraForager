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

                while (_streamCapacity < _tempQueue.Count)
                {
                    Hash128 hash = _tempQueue.Dequeue();
                    _streamedObjects.Remove(hash);
                }
            }
        }

        public int Count => _streamedObjects.Count;

        /// <summary>
        /// Current temporary queue offset affected by copies and already released references.
        /// </summary>
        public int Offset => _streamOffset;

        private HashSet<Hash128> _dirtyObjects = new HashSet<Hash128>();
        private Dictionary<Hash128, ScriptableObject> _streamedObjects = new Dictionary<Hash128, ScriptableObject>();
        private Dictionary<Hash128, short> _tempObjects = new Dictionary<Hash128, short>();
        private Queue<Hash128> _tempQueue = new Queue<Hash128>();

        private int _streamCapacity = 64;
        private int _streamOffset;

        public bool IsDirty(string id) => IsDirty(Hash128.Compute(id));
        public bool IsDirty(Hash128 hash) => _dirtyObjects.Contains(hash);

        /// <summary>
        /// Load temporary reference who can be released if not used and not set to dirty.
        /// </summary>
        public WeakReference<T>? LoadTemp<T>(string id) where T : ScriptableObject => LoadTemp<T>(Hash128.Compute(id));
        /// <summary>
        /// Load temporary reference that can be released if not used and not set to dirty.
        /// </summary>
        public WeakReference<T>? LoadTemp<T>(Hash128 hash) where T : ScriptableObject
        {
            if (_streamedObjects.ContainsKey(hash))
                return null;

            T obj = CreateInstance<T>();
            obj.name = hash.ToString();

            _streamedObjects.Add(hash, obj);
            _tempObjects.Add(hash, 1);

            _tempQueue.Enqueue(hash);
            if (_streamCapacity + _streamOffset < _tempQueue.Count)
                while (_tempQueue.Count > 0)
                {
                    Hash128 outHash = _tempQueue.Dequeue();

                    if (_tempObjects.ContainsKey(outHash) && --_tempObjects[outHash] > 0)   // If more copies exit inside of the queue.
                    {
                        _streamOffset--;
                        continue;
                    }

                    if (_streamedObjects.ContainsKey(outHash))
                    {
                        if (!_dirtyObjects.Contains(outHash))
                        {
                            Release(outHash);
                            break;
                        }
                    }

                    _streamOffset--;
                }

            return new WeakReference<T>(obj);
        }

        /// <summary>
        /// Load persistent reference that needs to be manually released. 
        /// </summary>
        public T? LoadPersistent<T>(string id) where T : ScriptableObject => LoadPersistent<T>(Hash128.Compute(id));
        /// <summary>
        /// Load persistent reference that needs to be manually released. 
        /// </summary>
        public T? LoadPersistent<T>(Hash128 hash) where T : ScriptableObject
        {
            if (_streamedObjects.ContainsKey(hash))
                return null;

            T obj = ScriptableObject.CreateInstance<T>();

            _streamedObjects.Add(hash, obj);

            return obj;
        }

        /// <summary>
        /// Release existing reference. Removes dirty.
        /// </summary>
        public bool Release(string id) => Release(Hash128.Compute(id));
        /// <summary>
        /// Release existing reference. Removes dirty.
        /// </summary>
        public bool Release(Hash128 hash)
        {
            if (!_streamedObjects.TryGetValue(hash, out ScriptableObject obj))
                return false;

            if (_tempObjects.TryGetValue(hash, out short amount))
            {
                for (int i = 0; i < amount; i++)
                        _streamOffset++;

                _tempObjects.Remove(hash);
            }

            Destroy(obj);

            _streamedObjects.Remove(hash);
            _dirtyObjects.Remove(hash);

            return true;
        }

        /// <summary>
        /// Return weak reference and if temporary, place it last in the queue.
        /// </summary>
        public WeakReference<T?> Get<T>(string id) where T : ScriptableObject => Get<T>(Hash128.Compute(id));
        /// <summary>
        /// Return weak reference and if temporary, place it last in the queue.
        /// </summary>
        public WeakReference<T?> Get<T>(Hash128 hash) where T : ScriptableObject
        {
            _streamedObjects.TryGetValue(hash, out ScriptableObject obj);

            if (_tempObjects.ContainsKey(hash))
            {
                _tempObjects[hash]++;   // Increase the amount of copies existing inside of the queue.
                _streamOffset++;
                _tempQueue.Enqueue(hash);
            }

            return new WeakReference<T?>(obj as T);  
        }

        /// <summary>
        /// Set dirty to save the asset on next saving.
        /// </summary>
        public void SetDirty(string id) => SetDirty(Hash128.Compute(id));
        /// <summary>
        /// Set dirty to save the asset on next saving.
        /// </summary>
        public void SetDirty(Hash128 hash)
        {
            _dirtyObjects.Add(hash);

            if (_tempObjects.ContainsKey(hash))
                _streamOffset++;
        }
    }
}
