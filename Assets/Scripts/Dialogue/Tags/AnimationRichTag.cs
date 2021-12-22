using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class AnimationRichTag : IRichTag
{
    private HumanoidAnimationBehaviour _animationBehaviour;
    private string _currentActor;
    private AnimationClip _previousClip;
    private bool _previousIsLooping;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void IRichTag.EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        string[] args = Regex.Replace(parameter, @"\s", "").Split(',');

        if (args.Length == 0)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Requires actor id and clip as an argument");
            return;
        }
        else if (args.Length == 1)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Requires clip as an argument");
            return;
        }
        else if (args.Length > 3)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Too many arguments");
            return;
        }

        string actorId = args[0];
        string clip = args[1];
        bool loop = args.Length > 2 ? bool.Parse(args[2]) : true;

        // Get humanoid animation behaviour.
        if (new string[] { "fiona", "player" }.Contains(actorId))
            _animationBehaviour = PlayerInput.GetPlayerByIndex(manager.PlayerIndex).GetComponentInChildren<HumanoidAnimationBehaviour>();
        else if (NPCManager.Instance.TryGetObject(actorId, out NPCBehaviour npc))
            _animationBehaviour = npc.GetComponentInChildren<HumanoidAnimationBehaviour>();

        // Play custom motion.wdd
        if (_animationBehaviour != null)
            if (new string[] { "null", "none", "empty" }.Contains(clip))
            {
                _previousIsLooping = _animationBehaviour.IsCustomMotionLooping;
                _previousClip = _animationBehaviour.CurrentCustomClip;
                _animationBehaviour.BlendCancelCustomMotion();
            }
            else
                _animationBehaviour.CustomMotionRaise(DialogueAnimations.Instance.Get(clip.ToSnakeCase()), loop: loop);
        else
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: No {nameof(HumanoidAnimationBehaviour)} found on given actor: {actorId}");
            return;
        }

        _currentActor = actorId;
    }

    void IRichTag.ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        if (manager.IsAutoCancelled)
            return;

        if (_previousClip != null)   // Replay previous animation if animation was stopped.
        {
            _animationBehaviour.CustomMotionRaise(_previousClip, loop: _previousIsLooping);
            _previousClip = null;
            return;
        }

        if (!manager.IsDialogueActive)  // Cancel animation if dialogue is no longer running.
        {
            _animationBehaviour?.BlendCancelCustomMotion();
            return;
        }

        manager.StartCoroutine(WaitCancel());
        _animationBehaviour = null;
        _currentActor = null;
    }

    IEnumerator IRichTag.ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        yield return null; 
    }

    private IEnumerator WaitCancel()
    {
        if (_currentActor == null)
            yield break;

        HumanoidAnimationBehaviour animationBehaviour = _animationBehaviour;
        string currentActor = _currentActor;

        yield return new WaitForFixedUpdate();

        if (_currentActor == null || _currentActor != currentActor)
            animationBehaviour?.BlendCancelCustomMotion();
    }
}
