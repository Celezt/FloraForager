using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CustomDialogueTag]
public class AudioTag : ITag
{
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

        SoundPlayer.Instance.Play(args.First(), 0, 0, 0, cooldown, loop);
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
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
        
        SoundPlayer.Instance.Stop(args.First());
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }
}
