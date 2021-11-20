using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class RodItem : IUse, IStar, IValue
{
    public float CatchSize => _catchSize;
    public float DragForce => _dragForce;
    public float DragDamp => _dragDamp;
    public float DragWeight => _weight;
    public float Bounciness => _bounciness;

    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    Stars IStar.Star { get; set; } = Stars.One;
    [OdinSerialize, PropertyOrder(int.MinValue + 2)]
    int IValue.BaseValue { get; set; }
    [OdinSerialize, PropertyOrder(int.MinValue + 3)]
    float IUse.Cooldown { get; set; } = 1;

    [Title("Rod Behaviour")]
    [SerializeField, Range(0, 1)]
    private float _catchSize = 0.2f;
    [SerializeField]
    private float _weight = 6.0f;
    [SerializeField]
    private float _dragForce = 100.0f;
    [SerializeField]
    private float _dragDamp = 10.0f;
    [SerializeField]
    private float _bounciness = 0.5f;
    [Space(10)]
    [SerializeField]
    private string _castSound = "cast_rod";
    [SerializeField]
    private string _hookedSound = "hooked_fish";
    [SerializeField]
    private string _catchSound = "catch_fish";
    [SerializeField]
    private float _stunCastDuration;
    [SerializeField]
    private float _onCastUse;
    [SerializeField]
    private float _stunCatchDuration;
    [SerializeField]
    private float _onCatchUse;
    [Space(10)]
    [SerializeField]
    private LayerMask _hitMask = LayerMask.NameToLayer("Grid");
    [SerializeField]
    private float _radius = 8.0f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _arc = 60.0f;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {

    }

    void IItem.OnUnequip(ItemContext context)
    {

    }

    void IItem.OnUpdate(ItemContext context)
    {

    }

    IEnumerator IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Cell cell;
        if ((cell = Grid.Instance.HoveredCell) == null || cell.Type != CellType.Water)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _hitMask);

        if (!MathUtility.PointInArc(hitInfo.point, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
            yield break;

        Inventory inventory = PlayerInput.GetPlayerByIndex(context.playerIndex).GetComponent<PlayerInfo>().Inventory;
        List<(int, int)> items = inventory.FindAllByLabels(ItemLabels.FishBait); // find all baits in inventory

        if (items.Count <= 0) // if no baits found, return
            yield break;

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

        int fishBaitIndex = items.First().Item1; // select first found bait to use

        FishBaitItem fishBait = ItemTypeSettings.Instance.ItemTypeChunk[inventory.Get(fishBaitIndex).ID].Behaviour as FishBaitItem;
        RodItem rodItem = ItemTypeSettings.Instance.ItemTypeChunk[context.id].Behaviour as RodItem;

        // -- ACTIONS --

        void Cast() // calls when rod is casted
        {
            UIStateVisibility.Instance.Hide("inventory");
            playerInput.DeactivateInput();

            PlayerMovement playerMovement = playerInput.GetComponent<PlayerMovement>();
            playerMovement.SetDirection((hitInfo.point - context.transform.position).normalized.xz());



            SoundPlayer.Instance.Play(_castSound);
        };
        void Hooked()
        {


            SoundPlayer.Instance.Play(_hookedSound);
        };
        void Catch()
        {
            UIStateVisibility.Instance.Show("inventory");
            playerInput.ActivateInput();



            SoundPlayer.Instance.Play(_catchSound);
        };

        // -- HOOK --

        Cast(); // cast the rod

        string hookedFish = string.Empty;

        void PlayHookAction()
        {
            FishHook.Instance.OnHook -= HookAction;
            FishHook.Instance.OnHook += HookAction;

            FishHook.Instance.OnPlay -= PlayHookAction;
        };
        void HookAction(string fishID) // player 
        {
            hookedFish = fishID;
            FishHook.Instance.OnHook -= HookAction;
        };

        FishHook.Instance.OnPlay += PlayHookAction;

        IEnumerator enumerator = FishHook.Instance.PlayHook(this, fishBait); // start hook game
        while (enumerator.MoveNext())
            yield return enumerator.Current;

        // if no fish available or no space for fish, break
        if (string.IsNullOrWhiteSpace(hookedFish) || inventory.FindEmptySpace(hookedFish) <= 0)
        {
            Catch();
            yield break;
        }

        Hooked(); // this means the player has successfully hooked a fish

        // -- FISHING --

        FishingManager fishingManager = FishingManager.GetByIndex(context.playerIndex);

        fishingManager.OnPlayCallback += PlayFishAction;
        fishingManager.StartFishing(hookedFish, context.id);

        void PlayFishAction()
        {
            fishingManager.OnCatchCallback -= CatchFishAction;
            fishingManager.OnFleeCallback -= FleeFishAction;
            fishingManager.OnCatchCallback += CatchFishAction;
            fishingManager.OnFleeCallback += FleeFishAction;

            fishingManager.OnPlayCallback -= PlayFishAction;
        };
        void CatchFishAction()
        {
            Catch();

            inventory.RemoveAt(fishBaitIndex, 1);
            inventory.Insert(new ItemAsset { ID = hookedFish, Amount = 1 });

            fishingManager.OnCatchCallback -= CatchFishAction;
        };
        void FleeFishAction()
        {
            Catch();

            inventory.RemoveAt(fishBaitIndex, 1);

            fishingManager.OnFleeCallback -= FleeFishAction;
        };

        yield break;
    }
}
