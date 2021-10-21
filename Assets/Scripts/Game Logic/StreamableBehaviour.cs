using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Sirenix.Serialization;
using System.IO;

[DisallowMultipleComponent, RequireComponent(typeof(GuidComponent))]
public class StreamableBehaviour : MonoBehaviour, IStreamableObject<StreamableBehaviour.Data>
{
    private Data _data;

    public class Data
    {
        public GameObjectData Transform;
    }

    public Data OnUnload() => _data = new Data();
    public void OnLoad(object state) => _data = state as Data;


    private Dictionary<Hash128, object> _streamables;

    private Guid _guid;

    public void Start()
    {
        _guid = GetComponent<GuidComponent>().Guid;

        //GameManager.Instance.Stream.LoadTemp(_guid);

        Unload();

        Debug.Log(_streamables[Hash128.Compute(GetType().ToString())]);

        Load(new StreamBundle { Streamables = _streamables });

    }

    public object Unload()
    {
        _streamables = new Dictionary<Hash128, object>();

        GetComponentsInChildren<IStreamableObject>().ForEach(x => _streamables.Add(Hash128.Compute(x.GetType().ToString()), ((IStreamableObject<object>)x).OnUnload()));

        _data.Transform = new GameObjectData
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Scale = transform.localScale,
            SceneIndex = SceneManager.GetActiveScene().buildIndex,
            Address = name,
        };

        byte[] bytes = SerializationUtility.SerializeValueWeak(_data, DataFormat.Binary);
        File.WriteAllBytes(GameManager.SaveWorldPath + "hej.wld", bytes);

        return new StreamBundle { Streamables = _streamables };
    }

    public void Load(object state)
    {
        StreamBundle bundle = (StreamBundle)state;

        foreach (IStreamableObject<object> stream in GetComponentsInChildren<IStreamableObject>())
        {
            string typeName = stream.GetType().ToString();

            byte[] bytes = File.ReadAllBytes(GameManager.SaveWorldPath + "hej.wld");
            object output = SerializationUtility.DeserializeValueWeak(bytes, DataFormat.Binary);

            Debug.Assert(output is Data);

            Data data = output as Data;
            Debug.Log(data.Transform.Position);
            Debug.Log(data.Transform.Rotation);
            Debug.Log(data.Transform.Scale);
            Debug.Log(data.Transform.SceneIndex);
            Debug.Log(data.Transform.Address);

            if (bundle.Streamables.TryGetValue(Hash128.Compute(typeName), out object value))
                stream.OnLoad(value);
        }
    }
}
