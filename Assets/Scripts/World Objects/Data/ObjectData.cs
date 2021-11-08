using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class ObjectData
{
    [SerializeField, HideInInspector]
    private Data _Data;

    [System.NonSerialized]
    protected GameObject _GameObject;

    [System.Serializable]
    private class Data
    {
        public string ID;

        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public string Address;
        public int SceneIndex;
        public bool IsAlive = true;
    }

    public virtual void OnBeforeSaving()
    {
        if (_GameObject != null)
        {
            _Data.Position = _GameObject.transform.localPosition;
            _Data.Rotation = _GameObject.transform.localRotation;
            _Data.Scale = _GameObject.transform.localScale;
        }
    }
    public virtual void OnLoad(GameObject gameObject)
    {
        if (!_Data.IsAlive)
            GameObject.Destroy(gameObject);
    }
    public virtual void OnSceneLoad()
    {
        if (SceneManager.GetActiveScene().buildIndex != _Data.SceneIndex)
            return;

        if (!string.IsNullOrWhiteSpace(_Data.Address))
        {
            Addressables.LoadAssetAsync<GameObject>(_Data.Address).Completed += (AsyncOperationHandle<GameObject> handle) =>
            {
                _GameObject = GameObject.Instantiate(handle.Result);

                _GameObject.transform.localPosition = _Data.Position;
                _GameObject.transform.localRotation = _Data.Rotation;
                _GameObject.transform.localScale = _Data.Scale;

                Addressables.Release(handle);
            };
        }
    }
    public virtual void Destroy()
    {
        if (!string.IsNullOrWhiteSpace(_Data.Address))
        {
            ObjectMaster.Instance.Remove(_Data.ID);
        }
        else
            _Data.IsAlive = false;

        if (_GameObject != null)
            GameObject.Destroy(_GameObject);
    }

    public ObjectData(string id, GameObject gameObject, string address, int sceneIndex)
    {
        _Data = new Data();

        _Data.ID = id;

        _Data.Position = gameObject.transform.localPosition;
        _Data.Rotation = gameObject.transform.localRotation;
        _Data.Scale = gameObject.transform.localScale;

        _Data.Address = address;
        _Data.SceneIndex = sceneIndex;

        _GameObject = gameObject;
    }
}
