using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public struct AudioTag : ITag
{
    public void Initalize(Taggable taggable)
    {

    }

    public void Action(Taggable taggable, string parameter)
    {
        SoundPlayer.Instance.Play(parameter);
    }
}
