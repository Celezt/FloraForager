using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomTag]
public struct AudioTag : ITag
{
    public void Initalize(Taggable taggable)
    {

    }

    public void Action(Taggable taggable, string parameter)
    {
        Debug.Log(parameter);
    }
}
