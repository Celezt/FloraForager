using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ChildOf : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private bool _inheritPosition = true;
    [SerializeField, Indent, ShowIf(nameof(_inheritPosition))] Vector3 _positionOffset = Vector3.zero;
    [SerializeField] private bool _inheritRotation = true;
    [SerializeField, Indent, ShowIf(nameof(_inheritRotation))] Quaternion _rotationOffset = Quaternion.identity;
    [SerializeField] private bool _inheritScale = true;
    [SerializeField, Indent, ShowIf(nameof(_inheritScale))] Vector3 _scaleOffset = Vector3.one;

    private void Update()
    {
        if (_parent == null)
            return;

        Vector3 targetPosition = _parent.position - _positionOffset;
        Quaternion targetRotation = _parent.rotation * _rotationOffset;

        if (_inheritPosition)
            transform.position = RotatePointAroundPivot(targetPosition, _parent.position, targetRotation);

        if (_inheritRotation)
            transform.rotation = targetRotation;

        if (_inheritScale)
            transform.localScale = Vector3.Scale(_parent.localScale, _scaleOffset);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        //Get a direction from the pivot to the point
        Vector3 dir = point - pivot;
        //Rotate vector around pivot
        dir = rotation * dir;
        //Calc the rotated vector
        point = dir + pivot;
        //Return calculated vector
        return point;
    }
}
