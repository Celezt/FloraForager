using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Sirenix.Serialization;
using System.IO;
using Sirenix.OdinInspector;

[DisallowMultipleComponent, RequireComponent(typeof(GuidComponent))]
public class StreamableBehaviour : MonoBehaviour, IStreamable<StreamableBehaviour.Data>
{
    [Button]
    public void SaveButton() => GameManager.SaveGame();

    [Button]
    public void LoadButton() => GameManager.LoadGame();

    private Data _data;

    public class Data
    {
        public GameObjectData Transform;
    }

    public Data OnUpload() => _data = new Data();
    public void OnLoad(object state)
    {
        Data data = state as Data;
        GameObjectData gameObjectData = data.Transform;

        transform.position = gameObjectData.Position;
        transform.rotation = gameObjectData.Rotation;
        transform.localScale = gameObjectData.Scale;

        _data = data;
    }

    private Guid _guid;

    public void Start()
    {
        _guid = GetComponent<GuidComponent>().Guid;

        UpLoad();

        InvokeRepeating(nameof(UpdateTransform), 0, 2);
    }

    public object UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        GetComponentsInChildren<IStreamable>().ForEach(x => streamables.Add(x.GetType().ToString(), ((IStreamable<object>)x).OnUpload()));

        GameManager.Stream.Load(_guid, streamables);

        return null;
    }

    public void Load()
    {
        foreach (IStreamable<object> stream in GetComponentsInChildren<IStreamable>())
        {
            string typeName = stream.GetType().ToString();

            Dictionary<string, object>  streamables = (Dictionary<string, object>)GameManager.Stream.Get(_guid);

            if (streamables.TryGetValue(typeName, out object value))
                stream.OnLoad(value);
        }
    }

    private void UpdateTransform()
    {
        _data.Transform = new GameObjectData
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Scale = transform.localScale,
            SceneIndex = SceneManager.GetActiveScene().buildIndex,
            Address = name,
        };
    }
}
