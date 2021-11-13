using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using MyBox;

[CreateAssetMenu(fileName = "NPC", menuName = "Game Data/NPC")]
public class NPCInfo : SerializedScriptableObject
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
    [OdinSerialize, ShowIf("_HasCommissions"), ListDrawerSettings(ShowItemCount = false, Expanded = true)]
    private CommissionInfo[] _CommissionsInfo;

    [Title("Dialogue")]
    [Title("Repeating Dialogue", Bold = false)]
    [SerializeField, HideLabel, InlineProperty]
    private DialogueElement _RepeatingDialogue;
    [Title("Initial Dialogue", Bold = false)]
    [SerializeField, Space(5), ListDrawerSettings(ShowItemCount = false, DraggableItems = false, Expanded = true)]
    private DialoguePriority[] _InitialDialogue;
    [Title("Relation Dialogue", Bold = false)]
    [SerializeField, Space(5), ListDrawerSettings(ShowItemCount = false, Expanded = true), ShowIf("_HasRelation")]
    private DialogueRelation[] _RelationDialogue;

    public string Name => _Name;

    public bool HasRelation => _HasRelation;
    public MinMaxFloat RelationRange => _RelationRange;
    public float StartRelation => _StartRelation;

    public bool HasCommissions => _HasCommissions;
    public CommissionInfo[] CommissionsData => _CommissionsInfo;

    public DialogueElement RepeatingDialogue => _RepeatingDialogue;
    public DialoguePriority[] InitialDialogue => _InitialDialogue;
    public DialogueRelation[] RelationDialogue => _RelationDialogue;
}
