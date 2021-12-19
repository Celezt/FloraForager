using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class AudioRichTag : IRichTag
{
    public void OnActive(Taggable taggable)
    {

    }

    void IRichTag.EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {

    }

    void IRichTag.ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {

    }

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    IEnumerator IRichTag.ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        yield return null;
    }
}
