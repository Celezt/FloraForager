using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Flora", menuName = "Scriptable Objects/Flora")]
[Serializable]
public class FloraInfo : SerializedScriptableObject
{
    [OdinSerialize, VerticalGroup, Required, LabelWidth(80)] 
    private string _Name;
    [OdinSerialize, VerticalGroup, TextArea(5, 30)] 
    private string _Description;

    [Title("Harvesting")]
    [OdinSerialize, VerticalGroup]
    private ItemLabels _ItemLabels;
    [OdinSerialize, VerticalGroup]
    private IHarvest _HarvestMethod;
    [OdinSerialize, VerticalGroup, PropertySpace(10), ListDrawerSettings(Expanded = true)] 
    private RewardPair[] _Rewards = new RewardPair[1];

    [Title("Growth")]
    [OdinSerialize, VerticalGroup, MinValue(0)] 
    private int _GrowTime = 0; // growth time in days
    [OdinSerialize, VerticalGroup, AssetSelector(Paths = "Assets/Models/Flora"), ListDrawerSettings(Expanded = true)] 
    private GameObject[] _Stages = new GameObject[1]; // Number of visual growth stages this flora has [0 = start, x = final]

    public string Name => _Name;
    public string Description => _Description;

    public ItemLabels ItemLabels => _ItemLabels;
    public IHarvest HarvestMethod => _HarvestMethod;
    public RewardPair[] Rewards => _Rewards;
    
    public int GrowTime => _GrowTime;
    public GameObject[] Stages => _Stages;
}
