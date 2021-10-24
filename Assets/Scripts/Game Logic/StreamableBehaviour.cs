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
public class StreamableBehaviour : MonoBehaviour, IStreamer, IStreamable<StreamableBehaviour.Data>
{
    [Button]
    public void SaveButton() => GameManager.SaveGame();

    [Button]
    public void LoadButton() => GameManager.LoadGame();

    private Data _data;

    private Guid _guid;

    public class Data
    {
        public string Address;
    }

    public Data OnUpload() => _data = new Data();
    public void OnLoad(object state) => _data = state as Data;
    void IStreamable.OnBeforeSaving() { }

    public void Start()
    {
        _guid = GetComponent<GuidComponent>().Guid;

        GameManager.AddStreamer(this);
    }

    public void UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        GetComponentsInChildren<IStreamable>().ForEach(x => streamables.Add(x.GetType().ToString(), ((IStreamable<object>)x).OnUpload()));

        GameManager.Stream.Load(_guid, streamables);
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

    public void BeforeSaving()
    {
        GetComponentsInChildren<IStreamable>().ForEach(x => ((IStreamable<object>)x).OnBeforeSaving());
    }

    private void OnDestroy()
    {
        GameManager.RemoveStreamer(this);
    }
}
