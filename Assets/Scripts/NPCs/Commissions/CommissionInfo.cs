using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Commission", menuName = "Scriptable Objects/Commission")]
public class CommissionInfo : SerializedScriptableObject
{
    [OdinSerialize, Required] 
    private string _Title;
    [OdinSerialize, TextArea(5, 30)] 
    private string _Description;

    [Title("Challenge")]
    [OdinSerialize] 
    private int _TimeLimit;
    [OdinSerialize, ListDrawerSettings(Expanded = true), Required]
    private IObjective[] _Objectives = new IObjective[] { new GatherObjective() };

    [Title("Relations")]
    [OdinSerialize] 
    private Relation _MinRelation = Relation.Neutral;
    [OdinSerialize] 
    private float _RewardRelations;
    [OdinSerialize] 
    private float _PenaltyRelations;

    [Title("Rewards")]
    [OdinSerialize, ListDrawerSettings(Expanded = true), Required] 
    private RewardPair[] _Rewards = new RewardPair[] { new RewardPair() };

    public string Title => _Title;
    public string Description => _Description;

    public int TimeLimit => _TimeLimit;
    public IObjective[] Objectives => _Objectives;

    public Relation MinRelation => _MinRelation;
    public float RewardRelations => _RewardRelations;
    public float PenaltyRelations => _PenaltyRelations;

    public RewardPair[] Rewards => _Rewards;
}
