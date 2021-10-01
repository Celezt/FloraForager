using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// entity who gives out commissions to the player (attach to NPC)
/// </summary>
public class CommissionGiver : MonoBehaviour
{
    [SerializeField] private CommissionData[] _CommissionsData; // data used to create commissions

    private Commission[] _Commissions; // all of the commissions stored in this giver

    private NPC _NPC;
    private RelationshipManager _Relations;

    public Commission[] Commissions => _Commissions;
    public NPC NPC => _NPC;
    public RelationshipManager Relations => _Relations;

    private void Awake()
    {
        _NPC = GetComponent<NPC>();
        _Relations = GetComponent<RelationshipManager>();

        _Commissions = new Commission[_CommissionsData.Length];
        for (int i = 0; i < _Commissions.Length; ++i)
        {
            _Commissions[i] = new Commission(_CommissionsData[i], this);
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_NPC.Selected)
            {
                CommissionGiverWindow.Instance.ShowCommissions(this);
                CommissionGiverWindow.Instance.Open();
            }
        }
    }
}
