using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class SkipRichTag : IRichTag
{
    private bool _originalSkip;
    private bool _isSkippable;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void IRichTag.EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        bool skippable = false;

        if (!string.IsNullOrWhiteSpace(parameter) && !bool.TryParse(parameter, out skippable))
        {
            Debug.LogError($"{ DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to bool");
            return;
        }

        _originalSkip = manager.IsSkippable;
        manager.IsSkippable = _isSkippable = skippable;
    }

    void IRichTag.ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        manager.IsSkippable = _originalSkip;
    }

    IEnumerator IRichTag.ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        yield return null;
    }
}
