using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Commission", menuName = "Scriptable Objects/Commission")]
public class CommissionData : SerializedScriptableObject
{
    [OdinSerialize] 
    private string _Title;
    [OdinSerialize, TextArea(5, 30)] 
    private string _Description;

    [Title("Challenge")]
    [OdinSerialize] 
    private int _TimeLimit;
    [OdinSerialize, Required]
    private ObjectiveData[] _ObjectivesData = new ObjectiveData[1];

    [Title("Relations")]
    [OdinSerialize] 
    private Relation _MinRelation;
    [OdinSerialize] 
    private float _RewardRelations;
    [OdinSerialize] 
    private float _PenaltyRelations;

    [Title("Rewards")]
    [OdinSerialize, Required] 
    private RewardPair[] _Rewards = new RewardPair[1];

    public string Title => _Title;
    public string Description => _Description;

    public int TimeLimit => _TimeLimit;
    public ObjectiveData[] ObjectivesData => _ObjectivesData;

    public Relation MinRelation => _MinRelation;
    public float RewardRelations => _RewardRelations;
    public float PenaltyRelations => _PenaltyRelations;

    public RewardPair[] Rewards => _Rewards;
}
