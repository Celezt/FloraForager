using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class StaminaTag : ITag
{
    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void ITag.EnterTag(Taggable taggable, string parameter)
    {

        if (!int.TryParse(parameter, NumberStyles.Integer, CultureInfo.InvariantCulture, out int staminaChange))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            return;
        }

        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        PlayerInput.GetPlayerByIndex(manager.PlayerIndex).GetComponent<PlayerStamina>().Stamina += staminaChange;
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {
   
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }
}
