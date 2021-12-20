using System.Globalization;
using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class PitchTag : ITag
{
    public void Initialize(Taggable taggable)
    {

    }

    /// <summary>
    /// pitch{float}
    /// </summary>
    public void EnterTag(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float pitch))
            (manager.TagTypes["audible"] as AudibleTag).Pitch = pitch;
    }

    public void ExitTag(Taggable taggable, string parameter)
    {

    }

    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }

    public void OnActive(Taggable taggable)
    {

    }
}