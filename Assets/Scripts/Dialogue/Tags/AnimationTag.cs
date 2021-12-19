using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class AnimationTag : ITag
{
    public void Initialize(Taggable taggable)
    {

    }

    public void EnterTag(Taggable taggable, string parameter)
    {
        parameter = Regex.Replace(parameter, @"\s", "");

        string[] args = parameter.Split(',');

        if (args.Length < 2)
        {
            Debug.LogError("insufficient amount of arguments");
            return;
        }

        string actor = args[0];
        string animation = args[1];
        bool loop = true;

        if (args.Length > 2)
            loop = bool.Parse(args[2]);

        foreach (KeyValuePair<string, GameObject> item in taggable.Unwrap<DialogueManager>().GetActors())
        {
            HumanoidAnimationBehaviour animationBehaviour;
            if ((animationBehaviour = item.Value.GetComponentInChildren<HumanoidAnimationBehaviour>()) != null)
            {
                if (string.Equals(actor, item.Key, System.StringComparison.InvariantCultureIgnoreCase))
                    animationBehaviour.CustomMotionRaise(DialogueAnimations.Instance.Get(animation), loop: loop);
                else
                    animationBehaviour.BlendCancelCustomMotion();
            }
            else
                Debug.LogError($"no HumanoidAnimationBehaviour found on given actor: {item.Key}");
        }
    }

    public void ExitTag(Taggable taggable, string parameter)
    {

    }

    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, int length, string parameter)
    {
        yield return null;
    }

    public void OnActive(Taggable taggable)
    {

    }
}
