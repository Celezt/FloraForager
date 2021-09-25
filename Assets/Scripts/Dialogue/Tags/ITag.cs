using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITag
{
    public void Initalize(Taggable taggable);
    public void Action(Taggable taggable, string parameter);
}
