using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Celezt.Time;
using UnityEngine.SceneManagement;

public class TransformBehaviour : MonoBehaviour, IStreamable<TransformBehaviour.Data>
{
    private Data _data;

    public class Data
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
    }

    Data IStreamable<Data>.OnUpload() => _data = new Data 
    { 
        Position = transform.position, 
        Rotation = transform.rotation, 
        Scale = transform.lossyScale 
    };

    void IStreamable.OnLoad(object state)
    {
        Data data = state as Data;

        transform.position = data.Position;
        transform.rotation = data.Rotation;
        transform.localScale = data.Scale;

        _data = data;
    }

    void IStreamable.OnBeforeSaving()
    {
        _data.Position = transform.position;
        _data.Rotation = transform.rotation;
        _data.Scale = transform.localScale;
    }
}
