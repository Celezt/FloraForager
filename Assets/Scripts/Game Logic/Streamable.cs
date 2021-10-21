using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent, RequireComponent(typeof(GuidComponent))]
public class Streamable : MonoBehaviour, IStreamableObject<StreamableData>
{
    private StreamableData _data;

    StreamableData IStreamableObject<StreamableData>.Data() => _data;

    private Dictionary<Hash128, IStreamable> _streamables = new Dictionary<Hash128, IStreamable>();

    private Guid _guid;

    public void Start()
    {
        _guid = GetComponent<GuidComponent>().Guid;

        _data.Transform = new TransformData
        {
            Position = transform.position,
            Rotation = transform.rotation,
            Scale = transform.localScale,
            SceneIndex = SceneManager.GetActiveScene().buildIndex,
        };

        GetComponents(typeof(IStreamableObject)).ForEach(x => _streamables.Add(Hash128.Compute(x.GetType().ToString()), ((IStreamableObject<IStreamable>)x).Data()));
        GetComponentsInChildren(typeof(IStreamableObject)).ForEach(x => _streamables.Add(Hash128.Compute(x.GetType().ToString()), ((IStreamableObject<IStreamable>)x).Data()));

        _streamables.Add(Hash128.Compute(this.GetType().ToString()), _data);

        Debug.Log(_streamables[Hash128.Compute(this.GetType().ToString())]);

        //GameManager.Instance.Stream.LoadTemp(_guid);
    }
}
