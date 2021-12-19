using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CustomDialogueTag]
public class SpeedRichTag : IRichTag
{
    void IRichTag.EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {

    }

    void IRichTag.ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {

    }

    void ITaggable.Initialize(Taggable taggable)
    {
        
    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    IEnumerator IRichTag.ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager dialogue = taggable.Unwrap<DialogueManager>();

        if (!float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            yield break;
        }

        yield return new WaitForSeconds(dialogue.AutoTextSpeed / speedMultiplier);
    }
}
