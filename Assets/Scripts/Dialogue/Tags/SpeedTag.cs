using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public struct SpeedTag : IHierarchyTag
{
    void ITaggable.Initialize(Taggable taggable)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        manager.SetDefaultTag(this, true, "1");
    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void ITag.EnterTag(Taggable taggable, string parameter)
    {

    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {

    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (!float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            yield return null;
        }

        yield return new WaitForSeconds(manager.AutoTextSpeed / speedMultiplier);
    }

    void IHierarchyTag.EnterChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {

    }

    void IHierarchyTag.ExitChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {

    }

    IEnumerator IHierarchyTag.ProcessChild(Taggable thisTaggable, Taggable childTaggable, int currentIndex, int length, string parameter)
    {
        DialogueManager manager = thisTaggable.Unwrap<DialogueManager>();

        if (!float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            yield return null;
        }

        yield return new WaitForSeconds(manager.AutoTextSpeed / speedMultiplier);
    }
}
