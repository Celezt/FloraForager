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
    private int _respawnTimeInDays = 2;
    [SerializeField, MinMaxRange(0.0f, 5.0f), ShowIf("@this._respawnableObject && this._saveIfDestroyed")]
    private MinMaxInt _randomRespawnTime = new MinMaxInt(0, 3);

    private Data _data;
    private Guid _guid;

    public class Data
    {
        public bool IsAlive = true;
    }

    public Data OnUpload() => _data = new Data();
    public void OnLoad(object state)
    {
        _data = state as Data;

        gameObject.SetActive(_data.IsAlive);
    }
    void IStreamable.OnBeforeSaving() 
    {

    }

    private void Awake()
    {
        _guid = GetComponent<GuidComponent>().Guid;

        ObjectRespawn.Instance.AddObject(_guid, gameObject);
    }

    private void OnEnable()
    {
        if (GameManager.Stream.StreamedData.ContainsKey(_guid))
            Load();

        GameManager.AddStreamer(this);
    }
    private void OnDisable()
    {
        if (gameObject.scene.isLoaded)
        {
            if (_saveIfDestroyed)
            {
                _data.IsAlive = false;
                
                if (_respawnableObject)
                    ObjectRespawn.Instance.Add(_guid, _respawnTimeInDays + _randomRespawnTime.RandomInRangeInclusive(), gameObject);
            }
        }

        GameManager.RemoveStreamer(this);
    }
    private void OnDestroy()
    {
        GameManager.RemoveStreamer(this);
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
}
