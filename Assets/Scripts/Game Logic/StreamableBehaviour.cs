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
    [SerializeField] 
    private bool _saveIfDestroyed = true;

    [Title("Respawn Behaviour")]
    [SerializeField, ShowIf("_saveIfDestroyed")]
    private bool _respawnableObject = false;
    [SerializeField, Min(1), ShowIf("@this._respawnableObject && this._saveIfDestroyed")]
    private int _respawnTimeInDays = 1;

    private Data _data;
    private Guid _guid;

    public class Data
    {
        public string Address;
        public bool IsAlive = true;
        public int SceneIndex;
    }

    public Data OnUpload() => _data = new Data();
    public void OnLoad(object state)
    {
        Data data = state as Data;

        if (!data.IsAlive)
            Destroy(gameObject);

        _data = data;
    }
    void IStreamable.OnBeforeSaving() 
    {
        _data.SceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void Awake()
    {
        _guid = GetComponent<GuidComponent>().Guid;
    }

    private void Start()
    {
        if (GameManager.Stream.StreamedData.ContainsKey(_guid))
            Load();

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

            Dictionary<string, object> streamables = (Dictionary<string, object>)GameManager.Stream.Get(_guid);

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
        if (!LoadScene.SceneIsLoading)
        {
            if (_saveIfDestroyed)
            {
                _data.IsAlive = false;

                if (_respawnableObject)
                    ObjectRespawn.Instance.Add(_guid, _respawnTimeInDays);
            }
        }

        GameManager.RemoveStreamer(this);
    }
}
