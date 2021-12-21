using System.Collections;
using UnityEngine;

[CustomDialogueTag]
public class AudibleTag : IHierarchyTag
{
    public float Pitch
    {
        get => _pitch;
        set => _pitch = value;
    }
    public bool RichTagRunning
    {
        get => _isRichTagRunning;
        set => _isRichTagRunning = value;
    }

    private float _pitch;
    private bool _audible;

    private bool _isTagRunning;
    private bool _isRichTagRunning;

    public void Initialize(Taggable taggable)
    {

    }

    public void OnActive(Taggable taggable)
    {
        _audible = false;
        _pitch = 1.0f;
    }

    /// <summary>
    /// audible{bool}
    /// </summary>
    public void EnterTag(Taggable taggable, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        bool.TryParse(parameter, out _audible);

        _isTagRunning = true;
    }

    public void ExitTag(Taggable taggable, string parameter)
    {
        _isTagRunning = false;
    }

    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        if (!_isRichTagRunning)
        {
            DialogueManager manager = taggable.Unwrap<DialogueManager>();

            if (_audible && currentIndex > 0)
            {
                string letter = manager.ParsedText[currentIndex - 1].ToString();

                if (SoundPlayer.Instance.TryGetSound(letter, out SoundPlayer.Sound sound))
                    SoundPlayer.Instance.Play(letter, 0, _pitch - sound.Pitch);
            }
        }

        yield return null;
    }

    public void EnterChild(Taggable thisTaggable, Taggable childTaggable, string thisParameter)
    {

    }

    public void ExitChild(Taggable thisTaggable, Taggable childTaggable, string parameter)
    {

    }

    public IEnumerator ProcessChild(Taggable thisTaggable, Taggable childTaggable, int currentIndex, int length, string parameter)
    {
        if (!_isTagRunning && !_isRichTagRunning && bool.TryParse(parameter, out bool audible) && audible)
        {
            DialogueManager manager = thisTaggable.Unwrap<DialogueManager>();

            if (currentIndex > 0)
            {
                string letter = manager.ParsedText[currentIndex - 1].ToString();

                if (SoundPlayer.Instance.TryGetSound(letter, out SoundPlayer.Sound sound))
                    SoundPlayer.Instance.Play(letter, 0, _pitch - sound.Pitch);
            }
        }

        yield return null;
    }
}