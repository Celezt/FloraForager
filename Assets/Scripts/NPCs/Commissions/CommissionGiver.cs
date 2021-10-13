using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// entity who gives out commissions to the player (attach to NPC)
/// </summary>
[RequireComponent(typeof(NPC), typeof(RelationshipManager))]
public class CommissionGiver : MonoBehaviour, IInteractable
{
    [SerializeField] private CommissionData[] _CommissionsData; // data used to create commissions

    private Commission[] _Commissions; // all of the commissions stored in this giver
    private NPC _NPC;

    public Commission[] Commissions => _Commissions;
    public NPC NPC => _NPC;

    public int Priority => 0;

    private void Awake()
    {
        _NPC = GetComponent<NPC>();

        _Commissions = new Commission[_CommissionsData.Length];
        for (int i = 0; i < _Commissions.Length; ++i)
        {
            _Commissions[i] = new Commission(_CommissionsData[i], this);
        }
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        CommissionGiverWindow.Instance.ShowCommissions(this);
        CommissionGiverWindow.Instance.Open();
    }
}
