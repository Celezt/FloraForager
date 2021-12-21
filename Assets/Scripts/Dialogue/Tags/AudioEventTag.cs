using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public class AudioEventTag : IEventTag
{
    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    IEnumerator IEventTag.OnTrigger(Taggable taggable, int index, string parameter)
    {
        if (!taggable.IsCancelled)
            SoundPlayer.Instance.Play(parameter);

        yield return null;
    }
}
