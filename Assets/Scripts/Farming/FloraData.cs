using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flora", menuName = "Flora")]
public class FloraData : ScriptableObject
{
    [Space(3), Header("Properties")]
    [SerializeField] private string _Name;
    [SerializeField, TextArea(3, 10)] private string _Description;
    [SerializeField] private RewardPair<string, int>[] _Rewards;

    [Space(3), Header("Growth")]
    [SerializeField, Min(0)] private int _GrowTime = 0; // growth time in days
    [SerializeField] private GameObject[] _Stages;      // Number of visual growth stages this flora has [0 = start, x = final]

    public string Name => _Name;
    public string Description => _Description;
    public RewardPair<string, int>[] Rewards => _Rewards;

    public int GrowTime => _GrowTime;
    public GameObject[] Stages => _Stages;
}

[Serializable]
public struct RewardPair<T1, T2>
{
    public T1 ItemID;
    public T2 Amount;
}
