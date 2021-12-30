using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CustomDialogueTag]
public class AudioTag : ITag
{
    private string _soundName;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void ITag.EnterTag(Taggable taggable, string parameter)
    {
        string[] args = Regex.Replace(parameter, @"\s", "").Split(',');

        if (args.Length == 0)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Requires a sound name");
            return;
        }
        else if (args.Length > 3)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Too many arguments");
            return;
        }

        float cooldown = 0.0f;
        bool loop = false;

        if (args.Length > 1)
            float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out cooldown);
        if (args.Length > 2)
            bool.TryParse(args[2], out loop);

        _soundName = args.First();

        SoundPlayer.Instance.Play(_soundName, 0, 0, 0, cooldown, loop);
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {
        if (taggable.Unwrap<DialogueManager>().IsAutoCancelled)
            return;

        SoundPlayer.Instance.Stop(_soundName);
        _soundName = string.Empty;
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }
}
