using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class CommissionTag : ITag
{
    /// <summary>
    /// commission{actor, action}
    /// </summary>
    public void Action(Taggable taggable, string parameter)
    {
        parameter = Regex.Replace(parameter, @"\s", "");

        int index = parameter.IndexOf(',');

        string actor = parameter.Substring(0, index);
        string action = parameter.Substring(index + 1, parameter.Length - index - 1);

        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (!manager.Aliases.TryGetValue(actor, out string npcName))
        {
            npcName = actor;
        }

        NPC npc = NPCManager.Instance.Get(npcName);

        if (npc != null)
        {
            if (!string.IsNullOrWhiteSpace(action))
            {
                if (action.ToLower() == "open")
                {
                    npc.OpenCommissions();
                }
                else if (action.ToLower() == "close")
                {
                    npc.CloseCommissions();
                }
            }
            else
                Debug.LogError($"{action} is not a valid action");
        }
        else
            Debug.LogError($"{npcName} does not exist among NPCs");
    }

    public void Initalize(Taggable taggable)
    {

    }
}
