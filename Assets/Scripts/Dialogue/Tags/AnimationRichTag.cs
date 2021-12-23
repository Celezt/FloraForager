using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class AnimationRichTag : IRichTag
{
    public Stack<string> CurrentActorsStack => _currentActorStack;

    private Stack<AnimationBehaviour> _animationBehaviourStack;
    private Stack<string> _currentActorStack;
    private Stack<AnimationClip> _previousClipStack;
    private Stack<bool> _previousIsLoopingStack;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {
        _animationBehaviourStack = new Stack<AnimationBehaviour>();
        _currentActorStack = new Stack<string>();
        _previousClipStack = new Stack<AnimationClip>();
        _previousIsLoopingStack = new Stack<bool>();
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

        if (!AnimationManager.Instance.Clips.ContainsKey(clip)) // If clip does not exist.
        {
            string closestClip = "";
            int closestDistance = int.MaxValue;
            foreach (string name in AnimationManager.Instance.Clips.Keys)
            {
                int distance = clip.LevenshteinDistance(name);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestClip = name;
                }
            }

            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: \"{clip}\" does not exit. Should it be \"{closestClip}\"?");
            return;
        }

        // Get humanoid animation behaviour.
        AnimationBehaviour animationBehaviour = null;
        if (new string[] { "fiona", "player" }.Contains(actorId))
            _animationBehaviourStack.Push(animationBehaviour = PlayerInput.GetPlayerByIndex(manager.PlayerIndex).GetComponentInChildren<AnimationBehaviour>());
        else if (NPCManager.Instance.TryGetObject(actorId, out NPCBehaviour npc))
            _animationBehaviourStack.Push(animationBehaviour = npc.GetComponentInChildren<AnimationBehaviour>());

        // Play custom motion.
        if (animationBehaviour != null)
        {
            if (new string[] { "null", "none", "empty" }.Contains(clip))
            {
                _previousIsLoopingStack.Push(animationBehaviour.IsCustomMotionLooping);
                _previousClipStack.Push(animationBehaviour.CurrentCustomClip);
                animationBehaviour.BlendCancelCustomMotion();
            }
            else
                animationBehaviour.CustomMotionRaise(AnimationManager.Instance.Clips[clip.ToSnakeCase()], loop: loop);
        }
        else
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: No {nameof(AnimationBehaviour)} found on given actor: {actorId}");
            return;
        }

        _currentActorStack.Push(actorId);
    }

    void IRichTag.ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        if (_previousClipStack.Count > 0)   // Replay previous animation if animation was stopped.
        {
            _animationBehaviourStack.Pop()?.CustomMotionRaise(_previousClipStack.Pop(), loop: _previousIsLoopingStack.Pop());
            _previousClipStack = null;
            return;
        }

        if (!manager.IsDialogueActive)  // Cancel animation if dialogue is no longer running.
        {
            _animationBehaviourStack.Pop()?.BlendCancelCustomMotion();
            return;
        }

        if (_currentActorStack.Count > 0)
            manager.StartCoroutine(WaitCancel(manager));
    }

    IEnumerator IRichTag.ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter)
    {
        yield return null; 
    }

    private IEnumerator WaitCancel(DialogueManager manager)
    {
        AnimationBehaviour animationBehaviour = _animationBehaviourStack.Pop();
        string currentActor = _currentActorStack.Pop();

        yield return new WaitForFixedUpdate();

        if ((manager.TagTypes["animation"] as AnimationTag).CurrentActorsQueue.Contains(currentActor))
            yield break;

        if (_currentActorStack.Count <= 0 || _currentActorStack.Contains(currentActor))
            animationBehaviour?.BlendCancelCustomMotion();
    }
}
