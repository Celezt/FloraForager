using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Flora", menuName = "Scriptable Objects/Flora")]
[Serializable]
public class FloraData : SerializedScriptableObject
{
    [OdinSerialize, VerticalGroup, Required, LabelWidth(80)] 
    private string _Name;
    [OdinSerialize, VerticalGroup, TextArea(5, 30)] 
    private string _Description;

    [Title("Rewards"), PropertySpace(15)]
    [OdinSerialize, VerticalGroup, ListDrawerSettings(Expanded = true)] 
    private RewardPair<string, int>[] _Rewards = new RewardPair<string, int>[1];

    [Title("Growth"), PropertySpace(15)]
    [OdinSerialize, VerticalGroup, MinValue(0)] 
    private int _GrowTime = 0; // growth time in days
    [OdinSerialize, VerticalGroup, AssetSelector(Paths = "Assets/Models/Flora"), ListDrawerSettings(Expanded = true)] 
    private GameObject[] _Stages = new GameObject[1]; // Number of visual growth stages this flora has [0 = start, x = final]

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
