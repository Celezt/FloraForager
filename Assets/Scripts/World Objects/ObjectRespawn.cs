using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

    public void Notify()
    {
        for (int i = _Objects.Count - 1; i >= 0; --i)
        {
            (Guid, int) item = _Objects[i];

            if (--item.Item2 <= 0)
            {
                if (GameManager.Stream.StreamedData.ContainsKey(item.Item1))
                    ((StreamableBehaviour.Data)GameManager.Stream.StreamedData[item.Item1]).IsAlive = true;

                _Objects.RemoveAt(i);
            }
            else
                _Objects[i] = item;
        }
    }

    public void UpLoad()
    {
        throw new NotImplementedException();
    }
    public void Load()
    {
        throw new NotImplementedException();
    }
    public void BeforeSaving()
    {
        throw new NotImplementedException();
    }
}
