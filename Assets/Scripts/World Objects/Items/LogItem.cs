using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class LogItem : IUse
{
    [OdinSerialize, PropertyOrder(int.MinValue)]
    public int ItemStack { get; set; } = 1;
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    public float Cooldown { get; set; } = 0.5f;

    [SerializeField, AssetList(Path = "Data/Dialogues"), AssetsOnly]
    private TextAsset _LogText;
    [SerializeField, Tooltip("[optional] add more aliases when reading the log")]
    private string[] _AdditionalAliases = new string[1];

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

    public IEnumerator OnUse(UseContext context)
    {
        if (!context.started)
            yield break;

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

        playerInput.DeactivateInput();

        string[] aliases = { context.name };
        if (_AdditionalAliases != null)
            aliases = aliases.Concat(_AdditionalAliases).ToArray();

        DialogueManager.GetByIndex(context.playerIndex).StartDialogue(_LogText.name, aliases).Completed += CompleteAction;

        void CompleteAction(DialogueManager manager)
        {
            playerInput.ActivateInput();
            manager.Completed -= CompleteAction;
        };
    }
}
