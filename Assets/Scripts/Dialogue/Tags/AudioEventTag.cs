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
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        if (!manager.IsAutoCancelled)
            SoundPlayer.Instance.Play(parameter);

        yield return null;
    }
}
