using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class AudioRichTag : IRichTag
{
    void IRichTag.EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        Debug.Log("enter tag");
    }

    void IRichTag.ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        Debug.Log("exit tag");
    }

    void ITaggable.Initalize(Taggable taggable)
    {
        Debug.Log("initialize");
    }

    IEnumerator IRichTag.ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        Debug.Log("process tag");

        yield return null;
    }
}
