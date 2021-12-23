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
    public Queue<string> CurrentActorsQueue => _currentActorQueue;

    private Queue<AnimationBehaviour> _animationBehaviourQueue;
    private Queue<ClipData> _clipDataQueue;
    private Queue<string> _currentActorQueue;
    private Queue<bool> _isRunningQueue;

    private struct ClipData
    {
        public string ClipName;
        public bool IsLoop;
    }

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {
        _animationBehaviourQueue = new Queue<AnimationBehaviour>();
        _clipDataQueue = new Queue<ClipData>();
        _currentActorQueue = new Queue<string>();
        _isRunningQueue = new Queue<bool>();
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
        string clip = args[1].ToSnakeCase();
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
            _animationBehaviourQueue.Enqueue(animationBehaviour = PlayerInput.GetPlayerByIndex(manager.PlayerIndex).GetComponentInChildren<AnimationBehaviour>());
        else if (NPCManager.Instance.TryGetObject(actorId, out NPCBehaviour npc))
            _animationBehaviourQueue.Enqueue(animationBehaviour = npc.GetComponentInChildren<AnimationBehaviour>());

        // Play custom motion.
        if (animationBehaviour != null)
            animationBehaviour.Play(AnimationManager.Instance.Clips[clip], loop: loop);
        else
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: No {nameof(AnimationBehaviour)} found on given actor: {actorId}");
            return;
        }

        _clipDataQueue.Enqueue(new ClipData { ClipName = clip, IsLoop = loop });
        _currentActorQueue.Enqueue(actorId);
        _isRunningQueue.Enqueue(true);
    }

    void ITag.ExitTag(Taggable taggable, string parameter)
    {       
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        if (manager.IsAutoCancelled)
        {
            ProcessAnimation(manager);
            return;
        }

        if (!manager.IsDialogueActive)  // Cancel animation if dialogue is no longer running.
        {
            _animationBehaviourQueue.Dequeue()?.Cancel();
            return;
        }

        if (_currentActorQueue.Count > 0)
            manager.StartCoroutine(WaitCancel(manager));
    }

    IEnumerator ITag.ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();
        ProcessAnimation(manager);

        yield return null;
    }

    private IEnumerator WaitCancel(DialogueManager manager)
    {
        AnimationBehaviour animationBehaviour = _animationBehaviourQueue.Dequeue();
        string currentActor = _currentActorQueue.Dequeue();
        _isRunningQueue.Clear();
        _clipDataQueue.Clear();

        yield return new WaitForFixedUpdate();

        if ((manager.RichTagTypes["animation"] as AnimationRichTag).CurrentActorsStack.Contains(currentActor) ||
            (manager.EventTagTypes["clip"] as ClipEventTag).IsPlaying)
            yield break;

        if (_currentActorQueue.Count <= 0 || !_currentActorQueue.Contains(currentActor))
            animationBehaviour?.Cancel();
    }

    private void ProcessAnimation(DialogueManager manager)
    {
        AnimationBehaviour animationBehaviour = _animationBehaviourQueue.Dequeue();
        ClipData clipData = _clipDataQueue.Dequeue();
        string actor = _currentActorQueue.Dequeue();
        bool isRunning = _isRunningQueue.Dequeue();

        if (!(manager.RichTagTypes["animation"] as AnimationRichTag).CurrentActorsStack.Contains(actor) &&
            !(manager.EventTagTypes["clip"] as ClipEventTag).IsPlaying)
        {
            if (!isRunning)
            {
                isRunning = true;
                animationBehaviour.Play(AnimationManager.Instance.Clips[clipData.ClipName], loop: clipData.IsLoop);
            }
        }
        else
            isRunning = false;

        _animationBehaviourQueue.Enqueue(animationBehaviour);
        _clipDataQueue.Enqueue(clipData);
        _currentActorQueue.Enqueue(actor);
        _isRunningQueue.Enqueue(isRunning);
    }
}
