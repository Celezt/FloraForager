using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[CustomDialogueTag]
public class WaitEventTag : IEventTag
{
    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    IEnumerator IEventTag.OnTrigger(Taggable taggable, int index, string parameter)
    {
        if (!float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            yield return null;
        }

        yield return new WaitForSeconds(speedMultiplier);
    }
}
