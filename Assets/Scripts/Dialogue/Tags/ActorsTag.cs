using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class ActorsTag : ITag
{
    public void Initialize(Taggable taggable)
    {

    }

    public void EnterTag(Taggable taggable, string parameter)
    {
        parameter = Regex.Replace(parameter, @"\s", "");

        string[] actors = parameter.Split(',');
        Dictionary<string, GameObject> actorObjects = new Dictionary<string, GameObject>();

        foreach (string actor in actors)
        {
            NPCBehaviour npc; PlayerInput player;
            if ((npc = NPCManager.Instance.GetObject(actor)) != null)
                actorObjects.Add(actor, npc.gameObject);
            else if ((string.Equals(actor, "fiona", System.StringComparison.InvariantCultureIgnoreCase) || 
                      string.Equals(actor, "player", System.StringComparison.InvariantCultureIgnoreCase)) &&
                      (player = PlayerInput.GetPlayerByIndex(0)) != null)
                actorObjects.Add(actor, player.gameObject);
        }

        if (actorObjects.Count <= 0)
        {
            Debug.LogError("no valid actors given");
            return;
        }

        taggable.Unwrap<DialogueManager>().SetActors(actorObjects);
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
