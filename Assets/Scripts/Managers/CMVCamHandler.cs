using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CMVCamHandler : MonoBehaviour
{
    public Vector3 Velocity => _velocity;
    public Vector3 Damping
    {
        get
        {
            CinemachineFramingTransposer framingTransposer = _cm.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (framingTransposer == null)
                throw new Exception(nameof(CinemachineFramingTransposer) + " was null");

            return new Vector3(framingTransposer.m_XDamping, framingTransposer.m_YDamping, framingTransposer.m_ZDamping);
        }
        set
        {
            CinemachineFramingTransposer framingTransposer = _cm.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (framingTransposer == null)
                throw new Exception(nameof(CinemachineFramingTransposer) + " was null");

            framingTransposer.m_XDamping = value.x;
            framingTransposer.m_YDamping = value.y;
            framingTransposer.m_ZDamping = value.z;
        }
    }

    [SerializeField] private CinemachineTargetGroup _cmFollow;
    [SerializeField] private CinemachineTargetGroup _cmLookAt;
    [SerializeField] private CinemachineVirtualCamera _cm;

    private List<Transform> _targets = new List<Transform>();

    private GameObject _empty;

    private Vector3 _oldPosition;
    private Vector3 _velocity;
    private bool _emptyIsUsed;
    private bool _isStarted;

    public int Count => _emptyIsUsed ? _targets.Count - 1 : _targets.Count;

    public Transform First => (_targets.Count > 0 && !_emptyIsUsed) ? _targets[0] : null;
    public Transform Last => (_targets.Count > 0 && !_emptyIsUsed) ? _targets.Last() : null;

    /// <summary>
    /// Remove all members.
    /// </summary>
    public void Clear()
    {
        if (_targets.Count > 0 && !_emptyIsUsed && _empty != null)
        {
            Transform firstTransform = First;
            _empty.transform.position = firstTransform.position;

            for (int i = 0; i < _targets.Count; i++)
            {
                _cmFollow.RemoveMember(_targets[i]);
                _cmLookAt.RemoveMember(_targets[i]);
            }

            _targets.Clear();

            AddMember(_empty.transform);
            _emptyIsUsed = true;
        }
    }

    public bool Contains(Transform trans) => _targets.Contains(trans);

    /// <summary>
    /// Add transform target to camera targets.
    /// </summary>
    /// <param name="trans"></param>
    /// <returns>If the transform does not already exist</returns>
    public bool AddMember(Transform trans)
    {
        if (_emptyIsUsed)
        {
            RemoveMember(_empty.transform);
            _emptyIsUsed = false;
        }

        if (!_targets.Contains(trans))
        {
            _targets.Add(trans);
            _cmFollow.AddMember(trans, 1, 0);
            _cmLookAt.AddMember(trans, 1, 0);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove transform target from camera targets.
    /// </summary>
    /// <param name="trans">Transform.</param>
    /// <returns>If transform exist.</returns>
    public bool RemoveMember(Transform trans)
    {
        if (!_emptyIsUsed && _targets.Count == 1 && _empty != null)
        {
            Transform firstTransform = First;
            _empty.transform.position = firstTransform.position;

            AddMember(_empty.transform);
            _emptyIsUsed = true;
        }

        if (_targets.Remove(trans))
        {
            _cmFollow.RemoveMember(trans);
            _cmLookAt.RemoveMember(trans);

            return true;
        }

        return false;
    }

    private void Awake()
    {
        for (int i = 0; i < _cmFollow.m_Targets.Length; i++)
            _targets.Add(_cmFollow.m_Targets[i].target);

        _empty = new GameObject("EmptyPivot");
    }

    private void Start()
    {
        _oldPosition = transform.position;
    }

    private void OnEnable()
    {
        if (Count == 0)
        {
            _empty.transform.position = transform.position;
            AddMember(_empty.transform);
            _emptyIsUsed = true;
        }
    }

    private void Update()
    {
        Vector3 position = transform.position;
        _velocity = (position - _oldPosition) / Time.deltaTime;
        _oldPosition = position;
    }

    private void OnDestroy()
    {
        Destroy(_empty);
    }
}
