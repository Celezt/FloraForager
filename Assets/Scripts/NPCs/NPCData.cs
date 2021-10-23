using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using MyBox;

[CreateAssetMenu(fileName = "NPC", menuName = "Scriptable Objects/NPC")]
public class NPCData : SerializedScriptableObject
{
    [OdinSerialize, Required]
    private string _Name;

    [Title("Relations")]
    [OdinSerialize]
    private bool _HasRelation;
    [OdinSerialize, MinMaxRange(-100f, 100f), ShowIf("_HasRelation")]
    private MinMaxFloat _RelationRange = new MinMaxFloat(-100f, 100f);
    [OdinSerialize, ShowIf("_HasRelation")]
    private float _StartRelation = 0f;

    [Title("Commissions")]
    [OdinSerialize]
    private bool _HasCommissions;
    [OdinSerialize, ShowIf("_HasCommissions"), ListDrawerSettings(Expanded = true)]
    private CommissionData[] _CommissionsData = new CommissionData[0];

    public string Name => _Name;

    public bool HasRelation => _HasRelation;
    public MinMaxFloat RelationRange => _RelationRange;
    public float StartRelation => _StartRelation;

    public bool HasCommissions => _HasCommissions;
    public CommissionData[] CommissionsData => _CommissionsData;
}
