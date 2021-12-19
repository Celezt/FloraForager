using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITag : ITaggable
{
    public void EnterTag(Taggable taggable, string parameter);
    public void ExitTag(Taggable taggable, string parameter);
    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, int length, string parameter);
}
