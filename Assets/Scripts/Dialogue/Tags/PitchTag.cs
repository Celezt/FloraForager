using System.Globalization;
using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class PitchTag : ITag
{
    /// <summary>
    /// pitch{float}
    /// </summary>
    public void Action(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float pitch))
            manager.SetPitch(pitch);
    }

    public void Initalize(Taggable taggable)
    {

    }
}