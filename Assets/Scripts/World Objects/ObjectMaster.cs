using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "ObjectMaster", menuName = "Game Logic/Object Master")]
[System.Serializable]
public class ObjectMaster : SerializedScriptableSingleton<ObjectMaster>, IStreamer
{
    [SerializeField]
    private System.Guid _Guid;

    [System.NonSerialized]
    private Dictionary<string, ObjectData> _Objects = new Dictionary<string, ObjectData>();

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
        foreach (KeyValuePair<string, ObjectData> item in _Objects)
        {
            if (!_Objects.TryGetValue(item.Key, out ObjectData value))
                continue;

            value.OnSceneLoad();
        }
    }

    /// <summary>
    /// First parameter will be used for ID
    /// </summary>
    /// <typeparam name="T">type of the desired object to create</typeparam>
    /// <param name="constructorParams">parameters of the desired object's constructor</param>
    /// <returns>The created instance of the type or the already existing instance</returns>
    public T Get<T>(params object[] constructorParams) where T : ObjectData
    {
        if (constructorParams.Length != typeof(T).GetConstructors().First().GetParameters().Length)
        {
            Debug.LogError("invalid arguments");
            return null;
        }    

        string id = constructorParams.FirstOrDefault() as string;

        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("invalid ID");
            return null;
        }

        if (_Objects.ContainsKey(id))
        {
            GameObject gameObject = constructorParams.ElementAtOrDefault(1) as GameObject;

            if (gameObject == null)
            {
                Debug.LogError("invalid GameObject");
                return null;
            }

            _Objects[id].OnLoad(gameObject);
            return _Objects[id] as T;
        }

        T data = (T)System.Activator.CreateInstance(typeof(T), constructorParams);

        return (_Objects[id] = data) as T;
    }

    public bool Remove(string id)
    {
        if (!_Objects.ContainsKey(id))
            return false;

        _Objects.Remove(id);

        return true;
    }

    public void UpLoad()
    {
        GameManager.Stream.Load(_Guid, _Objects);
    }
    public void Load()
    {
        _Objects.Clear();

        if (!GameManager.Stream.TryGet(_Guid, out Dictionary<string, ObjectData> objects))
            return;

        _Objects = objects;
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad();

        foreach (KeyValuePair<string, ObjectData> item in _Objects)
        {
            item.Value.OnBeforeSaving();
        }
    }
}
