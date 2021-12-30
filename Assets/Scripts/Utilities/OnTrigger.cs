using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class OnTrigger : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private UnityEvent _enterEvents;
    [SerializeField]
    private UnityEvent _exitEvents;

    private void OnTriggerEnter(Collider other)
    {
        if (_layerMask.Contains(other.gameObject.layer))
            _enterEvents.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_layerMask.Contains(other.gameObject.layer))
            _exitEvents.Invoke();
    }
}
