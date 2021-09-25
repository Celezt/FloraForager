using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomTag]
public struct AudioTag : ITag
{
    public void Initalize(Taggable taggable)
    {
        Debug.Log("init");
    }

    public void Action(Taggable taggable, string parameter)
    {
        Debug.Log(parameter);
    }
}
