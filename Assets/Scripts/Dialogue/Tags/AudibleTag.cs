using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class AudibleTag : ITag
{
    /// <summary>
    /// audible{bool}
    /// </summary>
    public void EnterTag(Taggable taggable,  string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (bool.TryParse(parameter, out bool audible))
            manager.SetAudible(audible);
    }

    public void ExitTag(Taggable taggable, string parameter)
    {
        
    }

    public void Initialize(Taggable taggable)
    {

    }

    public void OnActive(Taggable taggable)
    {

    }

    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }
}