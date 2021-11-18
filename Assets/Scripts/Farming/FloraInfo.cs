using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Flora", menuName = "Game Data/Flora")]
[Serializable]
public class FloraInfo : SerializedScriptableObject
{
    [OdinSerialize, Required, LabelWidth(80)] 
    private string _Name;
    [OdinSerialize, TextArea(5, 30)] 
    private string _Description;

    [Title("Harvesting")]
    [OdinSerialize]
    private string _HarvestSound = "break_flora";
    [OdinSerialize]
    private ItemLabels _ItemLabels;
    [OdinSerialize, VerticalGroup]
    private IHarvest _HarvestMethod;

    [Title("Growth")]
    [OdinSerialize, MinValue(0)] 
    private int _GrowTime = 0; // growth time in days
    [OdinSerialize, AssetSelector(Paths = "Assets/Models/Flora"), AssetsOnly, ListDrawerSettings(Expanded = true)] 
    private GameObject[] _Stages; // Number of visual growth stages this flora has [0 = start, x = final]

    public string Name => _Name;
    public string Description => _Description;

    public string HarvestSound => _HarvestSound;
    public ItemLabels ItemLabels => _ItemLabels;
    public IHarvest HarvestMethod => _HarvestMethod;
    
    public int GrowTime => _GrowTime;
    public GameObject[] Stages => _Stages;
}
