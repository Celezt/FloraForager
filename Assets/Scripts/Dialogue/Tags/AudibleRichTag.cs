using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class AudibleRichTag : IRichTag
{
    private float _pitch;
    private bool _audible;

    public void Initialize(Taggable taggable)
    {

    }

    public void OnActive(Taggable taggable)
    {
        _audible = false;
        _pitch = 1.0f;
    }

    public void EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        AudibleTag audibleTag = manager.TagTypes["audible"] as AudibleTag;

        if (string.IsNullOrEmpty(parameter) || bool.TryParse(parameter, out _audible) && _audible)
            _audible = true;

        _pitch = audibleTag.Pitch;
        audibleTag.RichTagRunning = true;
    }

    public void ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        (manager.TagTypes["audible"] as AudibleTag).RichTagRunning = false;
    }

    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        bool audible;
        if (string.IsNullOrEmpty(parameter) || (bool.TryParse(parameter, out audible) && audible))
        {
            if (_audible)
            {
                string letter = manager.ParsedText[currentIndex].ToString();

                if (SoundPlayer.Instance.TryGetSound(letter, out SoundPlayer.Sound sound))
                    SoundPlayer.Instance.Play(letter, 0, _pitch - sound.Pitch);
            }
        }

        yield return null;
    }
}