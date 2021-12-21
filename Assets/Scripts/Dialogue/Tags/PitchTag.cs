using System.Globalization;
using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class PitchTag : ITag
{
    private float _pitch;

    public void Initialize(Taggable taggable)
    {

    }

    /// <summary>
    /// pitch{float}
    /// </summary>
    public void EnterTag(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        AudibleTag audibleTag = manager.TagTypes["audible"] as AudibleTag;

        _pitch = audibleTag.Pitch;

        if (float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float pitch))
            audibleTag.Pitch = pitch;
    }

    public void ExitTag(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        (manager.TagTypes["audible"] as AudibleTag).Pitch = _pitch;
    }

    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }

    public void OnActive(Taggable taggable)
    {

    }
}