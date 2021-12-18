using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITag : ITaggable
{
    public void Action(Taggable taggable, string parameter);
}
