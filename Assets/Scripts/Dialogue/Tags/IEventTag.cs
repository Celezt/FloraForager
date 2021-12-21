using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventTag : ITaggable
{
    public IEnumerator OnTrigger(Taggable taggable, int index, string parameter);
}
