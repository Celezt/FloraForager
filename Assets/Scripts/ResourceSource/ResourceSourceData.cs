using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Resource Source", menuName = "Scriptable Objects/Resource Source")]
public class ResourceSourceData : SerializedScriptableObject
{
    [Title("Collecting")]
    [OdinSerialize] 
    private string _ItemID;             // item that will be given when harvesting this resource
    [OdinSerialize, Min(0)] 
    private int _Amount;                // amount of items stored in this source
    [OdinSerialize, Min(0)] 
    private int _AmountPerCollect;      // amount of items to give per collection, set to 0 to alternatively add all items when finished collecting
    [OdinSerialize, Min(0)] 
    private float _TotalCollectionTime; // time in seconds to collect all the resources in this resource

    [Title("Properties")]
    [OdinSerialize, Min(0)]
    private float _Strength;            // strength required to interact with this object

    public string ItemID => _ItemID;
    public int Amount => _Amount;
    public int AmountPerCollect => _AmountPerCollect;
    public float TotalCollectionTime => _TotalCollectionTime;

    public float Strength => _Strength;
}
