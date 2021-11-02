using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Commission", menuName = "Game Data/Commission")]
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
    private IObjective[] _Objectives;

    [Title("Relations")]
    [OdinSerialize] 
    private Relation _MinRelation = Relation.Neutral;
    [OdinSerialize] 
    private float _RewardRelations;
    [OdinSerialize] 
    private float _PenaltyRelations;

    [Title("Rewards")]
    [OdinSerialize, ListDrawerSettings(Expanded = true), Required] 
    private ItemAsset[] _Rewards;

    public string Title => _Title;
    public string Description => _Description;

    public int TimeLimit => _TimeLimit;
    public IObjective[] Objectives => _Objectives;

    public Relation MinRelation => _MinRelation;
    public float RewardRelations => _RewardRelations;
    public float PenaltyRelations => _PenaltyRelations;

    public ItemAsset[] Rewards => _Rewards;
}
