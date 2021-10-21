using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

[CreateAssetMenu(fileName = "GameManager", menuName = "Game Logic/Game Manager")]
[System.Serializable]
public class GameManager : SerializedScriptableSingleton<GameManager>
{
    public StreamData Stream => _stream;

    [NonSerialized]
    private StreamData _stream = new StreamData();

    private InitalizeGame _initalizeGame = new InitalizeGame();

    public class InitalizeGame
    {
        [RuntimeInitializeOnLoadMethod]
        static void Initalize()
        {
            GameManager gameManager = GameManager.Instance;
            gameManager._initalizeGame.LoadOrder();
        }

        private void LoadOrder()
        {
            void LoadPlayerData()
            {
                StreamData stream = GameManager.Instance.Stream;

                //stream.LoadPersistent<Inventory>("player_inventory_0");
                //stream.LoadPersistent<PlayerData>("player_data_0");
            }

            LoadPlayerData();
        }

    }

    public class StreamData
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

                ReleaseOld();
            }
        }

        public int Count => _streamedData.Count;

        /// <summary>
        /// Current temporary queue offset affected by copies and already released references.
        /// </summary>
        public int Offset => _streamOffset;

        private HashSet<Guid> _dirtyData = new HashSet<Guid>();
        private Dictionary<Guid, ScriptableObject> _streamedData = new Dictionary<Guid, ScriptableObject>();
        private Dictionary<Guid, short> _tempData = new Dictionary<Guid, short>();
        private Queue<Guid> _tempQueue = new Queue<Guid>();

        private int _streamCapacity = 64;
        private int _streamOffset;

        public bool IsDirty(string id) => IsDirty(Guid.Parse(id));
        public bool IsDirty(Guid guid) => _dirtyData.Contains(guid);

        /// <summary>
        /// Load temporary reference that can be released if not used and not set to dirty.
        /// </summary>
        public WeakReference<T>? LoadTemp<T>(Guid guid) where T : ScriptableObject
        {
            if (_streamedData.ContainsKey(guid))
                return null;

            T obj = CreateInstance<T>();
            obj.name = guid.ToString();

            _streamedData.Add(guid, obj);
            _tempData.Add(guid, 1);

            _tempQueue.Enqueue(guid);
            ReleaseOld();

            return new WeakReference<T>(obj);
        }

        /// <summary>
        /// Load persistent reference that needs to be manually released. 
        /// </summary>
        public T? LoadPersistent<T>(Guid guid) where T : ScriptableObject
        {
            if (_streamedData.ContainsKey(guid))
                return null;

            T obj = ScriptableObject.CreateInstance<T>();

            _streamedData.Add(guid, obj);

            return obj;
        }

        /// <summary>
        /// Release existing reference. Removes dirty.
        /// </summary>
        public bool Release(string id) => Release(Guid.Parse(id));
        /// <summary>
        /// Release existing reference. Removes dirty.
        /// </summary>
        public bool Release(Guid guid)
        {
            if (!_streamedData.TryGetValue(guid, out ScriptableObject obj))
                return false;

            if (_tempData.TryGetValue(guid, out short amount))
            {
                for (int i = 0; i < amount; i++)
                        _streamOffset++;

                _tempData.Remove(guid);
            }

            Destroy(obj);

            _streamedData.Remove(guid);
            _dirtyData.Remove(guid);

            return true;
        }

        /// <summary>
        /// Return weak reference and if temporary, place it last in the queue.
        /// </summary>
        public WeakReference<T?> Get<T>(Guid guid) where T : ScriptableObject
        {
            _streamedData.TryGetValue(guid, out ScriptableObject obj);

            if (_tempData.ContainsKey(guid))
            {
                _tempData[guid]++;   // Increase the amount of copies existing inside of the queue.
                _streamOffset++;
                _tempQueue.Enqueue(guid);
            }

            return new WeakReference<T?>(obj as T);  
        }

        /// <summary>
        /// Set dirty to save the asset on next saving.
        /// </summary>
        public void SetDirty(Guid guid)
        {
            _dirtyData.Add(guid);

            if (_tempData.ContainsKey(guid))
                _streamOffset++;
        }

        private void ReleaseOld()
        {
            if (_streamCapacity + _streamOffset < _tempQueue.Count)
                while (_tempQueue.Count > 0)
                {
                    Guid guid = _tempQueue.Dequeue();

                    if (_tempData.ContainsKey(guid) && --_tempData[guid] > 0)   // If more copies exit inside of the queue.
                    {
                        _streamOffset--;
                        continue;
                    }

                    if (_streamedData.ContainsKey(guid))
                    {
                        if (!_dirtyData.Contains(guid))
                        {
                            Release(guid);
                            break;
                        }
                    }

                    _streamOffset--;
                }
        }
    }
}
