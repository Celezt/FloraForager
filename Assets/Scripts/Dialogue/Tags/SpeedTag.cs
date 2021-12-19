using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomDialogueTag]
public struct SpeedTag : ITag, IHierarchyTag
{
    private List<float> _speedMultiplierHierarchy;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {
        _speedMultiplierHierarchy = new List<float>() { 1 };    // Speed 1 at layer 0 by default.
    }

    void ITag.EnterTag(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (!float.TryParse(parameter, NumberStyles.Float, CultureInfo.InvariantCulture, out float speedMultiplier))
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: {parameter} could not be parsed to float");
            return;
        }

        while (_speedMultiplierHierarchy.Count <= taggable.Layer)
            _speedMultiplierHierarchy.Add(float.NaN);

        _speedMultiplierHierarchy[taggable.Layer] = speedMultiplier;

        Debug.Log("enter " + taggable.Layer);
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {
        Debug.Log("exit " + taggable.Layer);
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        DialogueManager dm = taggable.Unwrap<DialogueManager>();
        Debug.Log("process " + taggable.Layer);
        yield return new WaitForSeconds(dm.AutoTextSpeed / _speedMultiplierHierarchy[taggable.Layer]);
    }

    void IHierarchyTag.EnterChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {
        Debug.Log("enter: " + childTaggable.Layer);
    }

    void IHierarchyTag.ExitChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {
        Debug.Log("exit: " + childTaggable.Layer);
    }

    IEnumerator IHierarchyTag.ProcessChild(Taggable thisTaggable, Taggable childTaggable, int currentIndex, int length, string parameter)
    {
        DialogueManager dm = thisTaggable.Unwrap<DialogueManager>();

        Debug.Log("update: " + childTaggable.Layer);
        float speedMultiplier = 1;

        for (int i = thisTaggable.Layer > _speedMultiplierHierarchy.Count - 1 ? _speedMultiplierHierarchy.Count - 1 : _speedMultiplierHierarchy.Count - 2; i >= 0; i--) // Use the last speed if current is ahead of the hierarchy.
        {
            speedMultiplier = _speedMultiplierHierarchy[i];

            if (speedMultiplier != float.NaN)   // Stop searching if layer contains speed tag.
                break;
        }

        yield return new WaitForSeconds(dm.AutoTextSpeed / speedMultiplier);
    }
}
