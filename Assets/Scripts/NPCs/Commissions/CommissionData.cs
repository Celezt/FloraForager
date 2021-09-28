using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Commission", menuName = "Commission")]
public class CommissionData : ScriptableObject
{
    [SerializeField] private string _Title;
    [SerializeField, TextArea(3, 5)] private string _Description;
    [SerializeField] private ObjectiveData[] _ObjectivesData;
    [SerializeField] private RewardPair<string, int>[] _Rewards;

    public string Title => _Title;
    public string Description => _Description;
    public ObjectiveData[] ObjectivesData => _ObjectivesData;
    public RewardPair<string, int>[] Rewards => _Rewards;
}
