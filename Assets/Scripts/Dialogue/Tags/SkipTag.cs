using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class SkipTag : IHierarchyTag
{
    private bool _originalSkip;
    private bool _isSkippable;
    private bool _isTag;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void ITag.EnterTag(Taggable taggable, string parameter)
    {
        _isTag = true;

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

    void ITag.ExitTag(Taggable taggable, string parameter)
    {
        _isTag = false;

        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        manager.IsSkippable = _originalSkip;
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }

    void IHierarchyTag.EnterChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {
        if (_isTag)
            return;

        DialogueManager manager = thisTaggable.Unwrap<DialogueManager>();

        bool skippable = false;

        if (!string.IsNullOrWhiteSpace(parameter) && !bool.TryParse(parameter, out skippable))
        {
            Debug.LogError($"{ DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to bool");
            return;
        }

        _originalSkip = manager.IsSkippable;
        manager.IsSkippable = _isSkippable = skippable;
    }

    void IHierarchyTag.ExitChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {
        if (_isTag)
            return;

        DialogueManager manager = thisTaggable.Unwrap<DialogueManager>();
        manager.IsSkippable = _originalSkip;
    }

    IEnumerator IHierarchyTag.ProcessChild(Taggable thisTaggable, Taggable childTaggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }
}
