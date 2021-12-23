using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

[CustomDialogueTag]
public class ClipEventTag : IEventTag
{
    public bool IsPlaying => _isPlaying;

    private AnimationClip _previousClip;
    private bool _previousIsLooping;
    private bool _isPlaying;

    void ITaggable.Initialize(Taggable taggable)
    {

    }

    void ITaggable.OnActive(Taggable taggable)
    {

    }

    IEnumerator IEventTag.OnTrigger(Taggable taggable, int index, string parameter)
    {
        DialogueManager manager = taggable.Unwrap<DialogueManager>();

        string[] args = Regex.Replace(parameter, @"\s", "").Split(',');

        if (args.Length == 0)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Requires actor id and clip as an argument");
            yield break;
        }
        else if (args.Length == 1)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Requires clip as an argument");
            yield break;
        }
        else if (args.Length > 3)
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: Too many arguments");
            yield break;
        }

        string actorId = args[0];
        string clip = args[1];

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
            yield break;
        }

        // Get humanoid animation behaviour.
        AnimationBehaviour animationBehaviour = null;
        if (new string[] { "fiona", "player" }.Contains(actorId))
            animationBehaviour = PlayerInput.GetPlayerByIndex(manager.PlayerIndex).GetComponentInChildren<AnimationBehaviour>();
        else if (NPCManager.Instance.TryGetObject(actorId, out NPCBehaviour npc))
            animationBehaviour = npc.GetComponentInChildren<AnimationBehaviour>();

        
        // Play custom motion.
        if (animationBehaviour != null)
        {

            _isPlaying = true;
            _previousClip = animationBehaviour.CurrentCustomClip;
            _previousIsLooping = animationBehaviour.IsLooping;
            animationBehaviour.Play(AnimationManager.Instance.Clips[clip.ToSnakeCase()], exitCallback: x =>
            {
                _isPlaying = false;
                if (_previousClip == null)
                    return;

                if (!manager.CurrentNode.Tags.Any(x => x.Name == "animation"))
                    animationBehaviour.Cancel();

                if (!(manager.TagTypes["animation"] as AnimationTag).CurrentActorsQueue.Contains(actorId) ||
                    manager.CurrentTextIndex >= manager.CurrentTextMaxLength - 1 ||
                    manager.IsAutoCancelled)
                    animationBehaviour.Play(_previousClip, loop: _previousIsLooping);
            });
        }
        else
        {
            Debug.LogError($"{DialogueUtility.DIALOGUE_EXCEPTION}: No {nameof(AnimationBehaviour)} found on given actor: {actorId}");
           yield break;
        }
    }
}
