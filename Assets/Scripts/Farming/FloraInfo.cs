using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Flora", menuName = "Game Data/Flora")]
[Serializable]
public class FloraInfo : SerializedScriptableObject
{
    [SerializeField, Required, LabelWidth(80)] 
    private string _Name;
    [SerializeField, TextArea(5, 30)] 
    private string _Description;

    [Title("Harvesting")]
    [SerializeField]
    private string _HarvestSound = "break_flora";
    [SerializeField]
    private ItemLabels _ItemLabels;
    [OdinSerialize, VerticalGroup]
    private IHarvest _HarvestMethod;

    [Title("Growth")]
    [SerializeField, MinValue(0)] 
    private int _GrowTime = 0; // growth time in days
    [SerializeField, AssetSelector(Paths = "Assets/Models/Flora"), AssetsOnly, ListDrawerSettings(Expanded = true)] 
    private GameObject[] _Stages; // Number of visual growth stages this flora has [0 = start, x = final]

    public string Name => _Name;
    public string Description => _Description;

    public string HarvestSound => _HarvestSound;
    public ItemLabels ItemLabels => _ItemLabels;
    public IHarvest HarvestMethod => _HarvestMethod;
    
    public int GrowTime => _GrowTime;
    public GameObject[] Stages => _Stages;
}
