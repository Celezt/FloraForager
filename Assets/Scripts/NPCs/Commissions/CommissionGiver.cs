using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// entity who gives out commissions to the player (attach to NPC)
/// </summary>
public class CommissionGiver : MonoBehaviour
{
    [SerializeField] private CommissionData[] _CommissionsData;

    private Commission[] _Commissions;

    private NPC _NPC;

    public Commission[] Commissions => _Commissions;

    private void Awake()
    {
        _NPC = GetComponent<NPC>();

        _Commissions = new Commission[_CommissionsData.Length];
        for (int i = 0; i < _Commissions.Length; ++i)
        {
            _Commissions[i] = new Commission(_CommissionsData[i]);
        }
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_NPC.Selected)
            {
                CommissionGiverWindow.Instance.ShowCommissions(this);
                CommissionGiverWindow.Instance.Open();
            }
        }
    }
}
