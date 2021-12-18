using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRichTag : ITaggable
{
    public void EnterTag(Taggable taggable, int currentIndex, RangeInt range, string parameter);
    public void ExitTag(Taggable taggable, int currentIndex, RangeInt range, string parameter);
    public IEnumerator ProcessTag(Taggable taggable, int currentIndex, RangeInt range, string parameter);

}
