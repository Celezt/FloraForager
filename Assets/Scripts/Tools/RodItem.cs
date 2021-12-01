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
    [SerializeField]
    private float _staminaChange = -0.05f;
    [Space(10)]
    [SerializeField]
    private string _castSound = "cast_rod";
    [SerializeField]
    private string _hookedSound = "hooked_fish";
    [SerializeField]
    private string _catchFishSound = "catch_fish";
    [SerializeField]
    private string _catchFailSound = "catch_fail";
    [SerializeField, AssetsOnly]
    private GameObject _model;
    [SerializeField]
    private AnimationClip _castClip;
    [SerializeField]
    private AnimationClip _idleClip;
    [SerializeField]
    private AnimationClip _hookClip;
    [SerializeField]
    private AnimationClip _catchClip;
    [SerializeField]
    private float _onSwing = 0.85f;
    [SerializeField]
    private float _onCatch = 0.1f;
    [SerializeField]
    private float _stunCastDuration = 1.2f;
    [SerializeField, Sirenix.OdinInspector.MinValue("_onSwing")]
    private float _onCastUse = 1.2f;
    [SerializeField]
    private float _stunCatchDuration = 1.2f;
    [SerializeField, Sirenix.OdinInspector.MinValue("_onCatch")]
    private float _onCatchUse = 1.2f;
    [Space(10)]
    [SerializeField]
    private float _radius = 4.0f;
    [SerializeField, Range(0.0f, 360.0f)]
    private float _arc = 80.0f;

    private PlayerStamina _playerStamina;

    void IItem.OnInitialize(ItemTypeContext context)
    {

    }

    void IItem.OnEquip(ItemContext context)
    {
        _playerStamina = context.transform.GetComponent<PlayerStamina>();
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
        if ((cell = GameGrid.Instance.HoveredCell) == null || cell.Type != CellType.Water)
            yield break;

        if (!MathUtility.PointInArc(GameGrid.Instance.MouseHit, context.transform.position, context.transform.localEulerAngles.y, _arc, _radius))
            yield break;

        Inventory inventory = PlayerInput.GetPlayerByIndex(context.playerIndex).GetComponent<PlayerInfo>().Inventory;
        List<(int, int)> items = inventory.FindAllByLabels(ItemLabels.FishBait); // find all baits in inventory

        if (items.Count <= 0) // if no baits found, return
            yield break;

        // -- START FISHING --

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

        UIStateVisibility.Instance.Hide("inventory");
        playerInput.DeactivateInput();

        HumanoidAnimationBehaviour animationBehaviour = playerInput.GetComponentInChildren<HumanoidAnimationBehaviour>();
        PlayerMovement playerMovement = playerInput.GetComponent<PlayerMovement>();

        playerMovement.SetDirection((GameGrid.Instance.MouseHit - context.transform.position).normalized.xz());

        int fishBaitIndex = items.First().Item1; // select first found bait to use

        FishBaitItem fishBait = ItemTypeSettings.Instance.ItemTypeChunk[inventory.Get(fishBaitIndex).ID].Behaviour as FishBaitItem;
        RodItem rodItem = ItemTypeSettings.Instance.ItemTypeChunk[context.id].Behaviour as RodItem;

        string fishStatus = _catchFailSound; // sound to play depending on current fish status (success/failed)

        GameObject model = null;
        SkinnedMeshRenderer rodMeshRenderer = null;

        IEnumerator exitEnumerator = Exit();

        IEnumerator Exit() // exit from fishing
        {
            playerMovement.ActivaInput.Add(_stunCatchDuration);
            animationBehaviour.CustomMotionRaise(_catchClip,
                enterCallback: info =>
                {
                    if (_model == null)
                        return;

                    model.transform.SetParent(info.animationBehaviour.HoldTransform);
                },
                exitCallback: info =>
                {
                    if (_model == null)
                        return;

                    Object.Destroy(model);
                });

            SoundPlayer.Instance.Stop(_hookedSound);

            yield return new WaitForSeconds(_onCatch);

            SoundPlayer.Instance.Play(fishStatus);

            yield return new WaitForSeconds(_onCatchUse - _onCatch);

            UIStateVisibility.Instance.Show("inventory");
            playerInput.ActivateInput();
        };

        // -- CAST ROD --

        playerMovement.ActivaInput.Add(_stunCastDuration);
        animationBehaviour.CustomMotionRaise(_castClip,
            enterCallback: info =>
            {
                if (_model == null)
                    return;

                model = Object.Instantiate(_model, info.animationBehaviour.HoldTransform);
                rodMeshRenderer = model.GetComponent<SkinnedMeshRenderer>();
            }
        );

        yield return new WaitForSeconds(_onSwing);

        SoundPlayer.Instance.Play(_castSound);
        _playerStamina.Stamina += _staminaChange;

        yield return new WaitForSeconds(_onCastUse - _onSwing);
        
        // -- IDLE LOOP --

        animationBehaviour.CustomMotionRaise(_idleClip, loop: true,
            enterCallback: info =>
            {
                if (_model == null)
                    return;

                model.transform.SetParent(info.animationBehaviour.HoldTransform);
            }
        );

        // -- HOOK MINIGAME --

        string hookedFish = null;

        void HookAction(string fishID)
        {
            hookedFish = fishID; // resulting fish from hook minigame
            FishHook.Instance.OnHook -= HookAction;
        };

        FishHook.Instance.OnHook += HookAction;

        IEnumerator hookEnumerator = FishHook.Instance.PlayHook(this, fishBait); // start hook game
        while (hookEnumerator.MoveNext())
            yield return hookEnumerator.Current;

        // if no fish available or no space for fish
        if (string.IsNullOrWhiteSpace(hookedFish) || inventory.FindEmptySpace(hookedFish) <= 0)
        {
            while (exitEnumerator.MoveNext())
                yield return exitEnumerator.Current;

            yield break;
        }

        SoundPlayer.Instance.Play(_hookedSound, 0, Random.Range(-0.2f, 0.5f), 0, Random.Range(3f, 4f), true);

        // -- FISHING --

        animationBehaviour.CustomMotionRaise(_hookClip, loop: true,
            enterCallback: info =>
            {
                if (_model == null)
                    return;

                model.transform.SetParent(info.animationBehaviour.HoldTransform);
            }
        );

        bool fishingDone = false;

        FishingManager fishingManager = FishingManager.GetByIndex(context.playerIndex);

        fishingManager.OnCatchCallback += CatchFishAction;
        fishingManager.OnFleeCallback += FleeFishAction;

        fishingManager.StartFishing(hookedFish, context.id);

        void CatchFishAction()
        {
            fishingDone = true;
            fishStatus = _catchFishSound;

            inventory.RemoveAt(fishBaitIndex, 1);
            inventory.Insert(new ItemAsset { ID = hookedFish, Amount = 1 });

            fishingManager.OnCatchCallback -= CatchFishAction;
            fishingManager.OnFleeCallback -= FleeFishAction;
        };
        void FleeFishAction()
        {
            fishingDone = true;
            fishStatus = _catchFailSound;

            inventory.RemoveAt(fishBaitIndex, 1);

            fishingManager.OnCatchCallback -= CatchFishAction;
            fishingManager.OnFleeCallback -= FleeFishAction;
        };

        while (!fishingDone) // while no fish has been caught
        {
            rodMeshRenderer.SetBlendShapeWeight(0, 100.0f * (0.5f + Mathf.PingPong(Time.time, 0.2f)));
            yield return null;
        }

        rodMeshRenderer.SetBlendShapeWeight(0, 0);

        while (exitEnumerator.MoveNext())
            yield return exitEnumerator.Current;

        yield break;
    }
}
