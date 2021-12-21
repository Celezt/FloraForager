using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class AudioTag : ITag
{
    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void ITag.EnterTag(Taggable taggable, string parameter)
    {
        SoundPlayer.Instance.Play(parameter);
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {

    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }
}
