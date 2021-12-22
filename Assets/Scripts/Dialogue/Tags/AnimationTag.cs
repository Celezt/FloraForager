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
    private Queue<HumanoidAnimationBehaviour> _animationBehaviourQueue;
    private Queue<string> _currentActorQueue;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {
        _animationBehaviourQueue = new Queue<HumanoidAnimationBehaviour>();
        _currentActorQueue = new Queue<string>();
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
        HumanoidAnimationBehaviour animationBehaviour = null;
        if (new string[] { "fiona", "player" }.Contains(actorId))
            _animationBehaviourQueue.Enqueue(animationBehaviour = PlayerInput.GetPlayerByIndex(manager.PlayerIndex).GetComponentInChildren<HumanoidAnimationBehaviour>());
        else if (NPCManager.Instance.TryGetObject(actorId, out NPCBehaviour npc))
            _animationBehaviourQueue.Enqueue(animationBehaviour = npc.GetComponentInChildren<HumanoidAnimationBehaviour>());

        // Play custom motion.
        if (animationBehaviour != null)
            animationBehaviour.CustomMotionRaise(DialogueAnimations.Instance.Get(clip), loop: loop);
        else
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: No {nameof(HumanoidAnimationBehaviour)} found on given actor: {actorId}");
            return;
        }

        _currentActorQueue.Enqueue(actorId);
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {       
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        if (manager.IsAutoCancelled)
            return;

        if (!manager.IsDialogueActive)  // Cancel animation if dialogue is no longer running.
        {
            _animationBehaviourQueue.Dequeue()?.BlendCancelCustomMotion();
            return;
        }

        manager.StartCoroutine(WaitCancel(manager));
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }

    private IEnumerator WaitCancel(DialogueManager manager)
    {
        if (_currentActorQueue.Count <= 0)
            yield break;

        HumanoidAnimationBehaviour animationBehaviour = _animationBehaviourQueue.Dequeue();
        string currentActor = _currentActorQueue.Dequeue();

        yield return new WaitForFixedUpdate();

        if ((manager.RichTagTypes["animation"] as AnimationRichTag).CurrentActorsStack.Contains(currentActor))
            yield break;

        if (_currentActorQueue.Count <= 0 || !_currentActorQueue.Contains(currentActor))
            animationBehaviour?.BlendCancelCustomMotion();
    }
}
