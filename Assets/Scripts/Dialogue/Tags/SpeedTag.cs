using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public struct SpeedTag : ITag
{
    public void Action(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (!float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            return;
        }

        manager.SetAutoTextSpeedMultiplier(taggable.Layer, speedMultiplier);
    }

    public void Initalize(Taggable taggable)
    {
        
    }
}
