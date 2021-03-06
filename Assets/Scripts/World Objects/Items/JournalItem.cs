using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class JournalItem : IUse
{
    [OdinSerialize, PropertyOrder(int.MinValue + 1)]
    float IUse.Cooldown { get; set; } = 0.5f;

    [SerializeField, AssetList(Path = "Data/Dialogues"), AssetsOnly]
    private TextAsset _JournalText;
    [SerializeField]
    private string[] _Aliases;

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

        context.behaviour.ApplyCooldown();

        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(context.playerIndex);

        playerInput.DeactivateInput();

        string[] aliases = { context.name };
        if (_Aliases != null && _Aliases.Length > 0)
            aliases = _Aliases;

        DialogueManager.GetByIndex(context.playerIndex).StartDialogue($"Journals/{_JournalText.name}.json", aliases).Completed += CompleteAction;

        void CompleteAction(DialogueManager manager)
        {
            playerInput.ActivateInput();
            manager.Completed -= CompleteAction;
        };
    }
}
