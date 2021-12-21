using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class AnimationTag : ITag
{
    private HumanoidAnimationBehaviour _animationBehaviour;
    private string _currentActor;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    void ITag.EnterTag(Taggable taggable, string parameter)
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

        // Play custom motion.
        if (_animationBehaviour != null)
            _animationBehaviour.CustomMotionRaise(DialogueAnimations.Instance.Get(clip), loop: loop);
        else
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: No {nameof(HumanoidAnimationBehaviour)} found on given actor: {actorId}");
            return;
        }

        _currentActor = actorId;
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {
        if (taggable.IsCancelled)
            return;

        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        manager.StartCoroutine(WaitCancel());
        _animationBehaviour = null;
        _currentActor = null;
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
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

        if (_currentActor != currentActor)
            animationBehaviour?.BlendCancelCustomMotion();
    }
}
