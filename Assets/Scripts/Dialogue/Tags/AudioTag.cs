using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public struct AudioTag : ITag
{
    public void Initialize(Taggable taggable)
    {

    }

    public void EnterTag(Taggable taggable, string parameter)
    {
        SoundPlayer.Instance.Play(parameter);
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
