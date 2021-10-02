using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Commission", menuName = "Scriptable Objects/Commission")]
public class CommissionData : ScriptableObject
{
    [SerializeField] private string _Title;
    [SerializeField, TextArea(3, 5)] private string _Description;
    [SerializeField] private int _TimeLimit;
    [SerializeField] private Relation _MinRelation;
    [SerializeField] private float _RewardRelations;
    [SerializeField] private float _PenaltyRelations;
    [SerializeField] private ObjectiveData[] _ObjectivesData;
    [SerializeField] private RewardPair<string, int>[] _Rewards;

    public string Title => _Title;
    public string Description => _Description;
    public int TimeLimit => _TimeLimit;
    public Relation MinRelation => _MinRelation;
    public float RewardRelations => _RewardRelations;
    public float PenaltyRelations => _PenaltyRelations;
    public ObjectiveData[] ObjectivesData => _ObjectivesData;
    public RewardPair<string, int>[] Rewards => _Rewards;
}
