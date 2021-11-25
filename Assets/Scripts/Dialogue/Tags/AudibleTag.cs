using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class AudibleTag : ITag
{
    /// <summary>
    /// audible{bool}
    /// </summary>
    public void Action(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (bool.TryParse(parameter, out bool audible))
            manager.SetAudible(audible);
    }

    public void Initalize(Taggable taggable)
    {

    }
}