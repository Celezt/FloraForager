using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBehaviour : MonoBehaviour, IStreamable<ContainerBehaviour.Data>, IInteractable
{
    private Data _Data;

    public int Priority => 1;

    public class Data
    {

    }
    public Data OnUpload() => _Data;
    public void OnLoad(object state)
    {
        _Data = state as Data;
    }
    public void OnBeforeSaving()
    {

    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void OnInteract(InteractContext context)
    {

    }

}
