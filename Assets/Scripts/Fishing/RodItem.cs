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

    [OdinSerialize, PropertyOrder(int.MinValue)]
    int IItem.ItemStack { get; set; } = 1;
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
    private float _Radius = 8.0f;
    [SerializeField]
    private float _Arc = 1.5f;

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

    IEnumerable<IUsable> IUse.OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        Cell cell;
        if ((cell = Grid.Instance.HoveredCell) == null || cell.Type != CellType.Water)
            yield break;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.NameToLayer("Grid"));

        if (!MathUtility.PointInArc(hitInfo.point, context.transform.position, context.transform.localEulerAngles.y, _Arc, _Radius))
            yield break;

        Inventory inventory = PlayerInput.GetPlayerByIndex(context.playerIndex).GetComponent<PlayerInfo>().Inventory;
        List<(int, int)> items = inventory.FindAllByLabels(ItemLabels.FishBait); // find all baits in inventory

        if (items.Count <= 0) // if no baits found, return
            yield break;

        int fishBaitIndex = items.First().Item1; // select first found bait to use

        FishBait fishBait = ItemTypeSettings.Instance.ItemTypeChunk[inventory.Get(fishBaitIndex).ID].Behaviour as FishBait;
        RodItem rodItem = ItemTypeSettings.Instance.ItemTypeChunk[context.id].Behaviour as RodItem;

        string fishID = FishPool.Instance.GetFish(fishBait);

        if (string.IsNullOrWhiteSpace(fishID) || inventory.FindEmptySpace(fishID) <= 0) // if no fish available or no space for fish, return
            yield break;

        FishingManager fishingManager = FishingManager.GetByIndex(context.playerIndex);

        fishingManager.OnPlayCallback += PlayAction;
        fishingManager.StartFishing(fishID, context.id);

        void PlayAction()
        {
            PlayerInput.GetPlayerByIndex(context.playerIndex).DeactivateInput();

            fishingManager.OnCatchCallback -= CatchAction;
            fishingManager.OnFleeCallback -= FleeAction;
            fishingManager.OnCatchCallback += CatchAction;
            fishingManager.OnFleeCallback += FleeAction;

            fishingManager.OnPlayCallback -= PlayAction;
        };
        void CatchAction()
        {
            PlayerInput.GetPlayerByIndex(context.playerIndex).ActivateInput();

            inventory.RemoveAt(fishBaitIndex, 1);
            inventory.Insert(new ItemAsset { ID = fishID, Amount = 1 });

            fishingManager.OnCatchCallback -= CatchAction;
        };
        void FleeAction()
        {
            PlayerInput.GetPlayerByIndex(context.playerIndex).ActivateInput();

            inventory.RemoveAt(fishBaitIndex, 1);

            fishingManager.OnFleeCallback -= FleeAction;
        };

        yield break;
    }
}
