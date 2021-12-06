using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyBox;

[CreateAssetMenu(fileName = "ObjectRespawn", menuName = "Game Logic/Object Respawn")]
[System.Serializable]
public class ObjectRespawn : SerializedScriptableSingleton<ObjectRespawn>, IStreamer
{
    [SerializeField]
    private Guid _Guid;

    [System.NonSerialized]
    private Dictionary<Guid, int> _ObjectsRespawn = new Dictionary<Guid, int>();
    [System.NonSerialized]
    private Dictionary<Guid, GameObject> _Objects = new Dictionary<Guid, GameObject>();

    public IReadOnlyDictionary<Guid, int> ObjectsRespawn => _ObjectsRespawn;
    public IReadOnlyDictionary<Guid, GameObject> Objects => _Objects;

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        GameManager.AddStreamer(Instance);
        SceneManager.sceneLoaded += Instance.OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        List<Guid> badKeys = _Objects.Where(pair => pair.Value == null).Select(pair => pair.Key).ToList();
        foreach (Guid key in badKeys)
        {
            _Objects.Remove(key);
        }
    }

    public void Add(Guid guid, int days, GameObject gameObject)
    {
        if (!_ObjectsRespawn.ContainsKey(guid))
             _ObjectsRespawn.Add(guid, days);
        if (!_Objects.ContainsKey(guid))
            _Objects.Add(guid, gameObject);
    }

    public void AddObject(Guid guid, GameObject gameObject)
    {
        if (_ObjectsRespawn.ContainsKey(guid) && !_Objects.ContainsKey(guid))
        {
            _Objects[guid] = gameObject;
        }
    }

    public void Notify()
    {
        foreach (Guid guid in _ObjectsRespawn.Keys.ToList())
        {
            int daysLeft = _ObjectsRespawn[guid];

            if (--daysLeft <= 0)
            {
                if (GameManager.Stream.TryGet(guid, out Dictionary<string, object> streamables))
                {
                    if (streamables.TryGetValue(typeof(StreamableBehaviour).ToString(), out object streamable))
                        (streamable as StreamableBehaviour.Data).IsAlive = true;
                    if (streamables.TryGetValue(typeof(TreeBehaviour).ToString(), out object tree))
                        (tree as TreeBehaviour.Data).Durability = (tree as TreeBehaviour.Data).MaxDurability;
                    if (streamables.TryGetValue(typeof(RockBehaviour).ToString(), out object rock))
                        (rock as RockBehaviour.Data).Durability = (rock as RockBehaviour.Data).MaxDurability;
                }
                if (_Objects.TryGetValue(guid, out GameObject gameObject))
                {
                    if (gameObject.TryGetComponent(out StreamableBehaviour streamableBehaviour))
                        streamableBehaviour.Load();
                }

                _ObjectsRespawn.Remove(guid);
                _Objects.Remove(guid);
            }
            else
                _ObjectsRespawn[guid] = daysLeft;
        }
    }

    public void UpLoad()
    {
        GameManager.Stream.Load(_Guid, _ObjectsRespawn);
    }
    public void Load()
    {
        _ObjectsRespawn.Clear();

        if (!GameManager.Stream.TryGet(_Guid, out Dictionary<Guid, int> streamables))
            return;

        _ObjectsRespawn = streamables;
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();
    }
}
