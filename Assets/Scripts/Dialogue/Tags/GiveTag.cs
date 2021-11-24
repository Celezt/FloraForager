using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomDialogueTag]
public class GiveTag : ITag
{
    public void Action(Taggable taggable, string parameter) // parameter (id,amount)
    {
        parameter = Regex.Replace(parameter, @"\s", "");

        int index = parameter.IndexOf(',');

        string itemID = parameter.Substring(0, index);
        string itemAmount = parameter.Substring(index + 1, parameter.Length - index - 1);
        PlayerInput playerInput = PlayerInput.GetPlayerByIndex(0);
        playerInput.GetComponent<PlayerInfo>().Inventory.Insert(itemID,int.Parse(itemAmount));
    }

    public void Initalize(Taggable taggable)
    {
        
    }
}
