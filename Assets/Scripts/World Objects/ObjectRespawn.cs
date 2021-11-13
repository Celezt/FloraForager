using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MyBox;

[CreateAssetMenu(fileName = "ObjectRespawn", menuName = "Game Logic/Object Respawn")]
[System.Serializable]
public class ObjectRespawn : SerializedScriptableSingleton<ObjectRespawn>, IStreamer
{
    [SerializeField]
    private Guid _Guid;

    [System.NonSerialized]
    private List<(Guid, int)> _Objects = new List<(Guid, int)>();

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();

        GameManager.AddStreamer(this);
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void Initialize()
    {
        GameManager.AddStreamer(Instance);
    }
#endif

    public void Add(Guid guid, int days)
    {
        _Objects.Add((guid, days));
    }

    public void Notify()
    {
        for (int i = _Objects.Count - 1; i >= 0; --i)
        {
            (Guid, int) item = _Objects[i];

            if (--item.Item2 <= 0)
            {
                if (GameManager.Stream.TryGet(item.Item1, out Dictionary<string, object> streamables))
                {
                    string typeName = typeof(StreamableBehaviour).ToString();
                    if (streamables.TryGetValue(typeName, out object value))
                    {
                        (value as StreamableBehaviour.Data).IsAlive = true;
                    }
                }

                _Objects.RemoveAt(i);
            }
            else
                _Objects[i] = item;
        }
    }

    public void UpLoad()
    {
        GameManager.Stream.Load(_Guid, _Objects);
    }
    public void Load()
    {
        _Objects.Clear();

        if (!GameManager.Stream.TryGet(_Guid, out List<(Guid, int)> streamables))
            return;

        _Objects = streamables;
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();
    }
}
