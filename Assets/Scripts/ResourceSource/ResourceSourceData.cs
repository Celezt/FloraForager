using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource Source", menuName = "Scriptable Objects/Resource Source")]
public class ResourceSourceData : ScriptableObject
{
    [SerializeField] private string _ItemID;             // item that will be given when harvesting this resource
    [SerializeField] private string _RequiredTool;       // tool that is required to interact with this resource
    [SerializeField] private int _Amount;                // amount of items stored in this resource source
    [SerializeField] private int _AmountPerCollect;      // amount of items to give per collection
    [SerializeField] private float _TotalCollectionTime; // time in seconds to collect all the resources in this resource

    public string ItemID => _ItemID;
    public string RequiredTool => _RequiredTool;
    public int Amount => _Amount;
    public int AmountPerCollect => _AmountPerCollect;
    public float TotalCollectionTime => _TotalCollectionTime;
}
